const API_BASE = "http://localhost:5080/api";
let selectedTests = [];

document.addEventListener("DOMContentLoaded", () => {
    loadClients();
    loadTests();

    document.getElementById("searchButton").addEventListener("click", loadTests);
    document.getElementById("testSearch").addEventListener("keyup", (e) => {
        if (e.key === "Enter") loadTests();
    });
    document.getElementById("saveButton").addEventListener("click", saveWorkOrder);
});

async function loadClients() {
    try {
        const response = await fetch(`${API_BASE}/clients`);
        if (!response.ok) throw new Error("Unable to load clients.");

        const clients = await response.json();
        const select = document.getElementById("clientId");

        select.innerHTML = '<option value="">Select Client</option>';

        clients.forEach(client => {
            select.innerHTML += `
                <option value="${client.clientId}">
                    ${escapeHtml(client.clientName)}
                </option>`;
        });
    } catch (error) {
        showMessage(error.message, "danger");
    }
}

async function loadTests() {
    try {
        const search = document.getElementById("testSearch").value.trim();
        const response = await fetch(
            `${API_BASE}/testmaster?search=${encodeURIComponent(search)}`
        );

        if (!response.ok) throw new Error("Unable to load tests.");

        const tests = await response.json();
        const tbody = document.getElementById("testTable");
        tbody.innerHTML = "";

        tests.forEach(test => {
            const alreadySelected = selectedTests.some(x => x.testId === test.testId);

            tbody.innerHTML += `
                <tr>
                    <td>${escapeHtml(test.testName)}</td>
                    <td>${escapeHtml(test.testCode)}</td>
                    <td>₹${Number(test.rate).toFixed(2)}</td>
                    <td>
                        <button class="btn btn-sm btn-primary test-add"
                                ${alreadySelected ? "disabled" : ""}
                                onclick="addTest(${test.testId}, '${escapeJs(test.testName)}', ${test.rate})">
                            ${alreadySelected ? "Added" : "Add"}
                        </button>
                    </td>
                </tr>`;
        });
    } catch (error) {
        showMessage(error.message, "danger");
    }
}

function addTest(testId, testName, rate) {
    if (selectedTests.some(x => x.testId === testId)) return;

    selectedTests.push({
        testId,
        testName,
        rate: Number(rate),
        quantity: 1
    });

    renderSelectedTests();
    loadTests();
}

function removeTest(testId) {
    selectedTests = selectedTests.filter(x => x.testId !== testId);
    renderSelectedTests();
    loadTests();
}

function updateQuantity(testId, quantity) {
    const item = selectedTests.find(x => x.testId === testId);
    if (!item) return;

    const value = Math.max(1, parseInt(quantity) || 1);
    item.quantity = value;

    renderSelectedTests();
}

function renderSelectedTests() {
    const tbody = document.getElementById("selectedTests");
    tbody.innerHTML = "";

    let total = 0;

    selectedTests.forEach(item => {
        const amount = item.rate * item.quantity;
        total += amount;

        tbody.innerHTML += `
            <tr>
                <td>${escapeHtml(item.testName)}</td>
                <td>₹${item.rate.toFixed(2)}</td>
                <td>
                    <input type="number"
                           min="1"
                           value="${item.quantity}"
                           class="form-control"
                           onchange="updateQuantity(${item.testId}, this.value)">
                </td>
                <td>₹${amount.toFixed(2)}</td>
                <td>
                    <button class="btn btn-sm btn-danger"
                            onclick="removeTest(${item.testId})">
                        Remove
                    </button>
                </td>
            </tr>`;
    });

    document.getElementById("totalAmount").textContent = total.toFixed(2);
}

async function saveWorkOrder() {
    clearMessage();

    const clientId = parseInt(document.getElementById("clientId").value);
    const patientName = document.getElementById("patientName").value.trim();
    const age = parseInt(document.getElementById("age").value);
    const gender = parseInt(document.getElementById("gender").value);
    const mobileNumber = document.getElementById("mobileNumber").value.trim();

    if (!clientId) {
        showMessage("Please select a client.", "danger");
        return;
    }

    if (!patientName) {
        showMessage("Please enter patient name.", "danger");
        return;
    }

    if (isNaN(age) || age < 0 || age > 120) {
        showMessage("Please enter a valid age.", "danger");
        return;
    }

    if (!/^[0-9]{10,15}$/.test(mobileNumber)) {
        showMessage("Mobile number must contain 10 to 15 digits.", "danger");
        return;
    }

    if (selectedTests.length === 0) {
        showMessage("Please select at least one test.", "danger");
        return;
    }

    const payload = {
        clientId,
        patientName,
        age,
        gender,
        mobileNumber,
        tests: selectedTests.map(x => ({
            testId: x.testId,
            quantity: x.quantity
        }))
    };

    try {
        const response = await fetch(`${API_BASE}/workorders`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(payload)
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(data.message || "Unable to save work order.");
        }

        displayResult(data);
        showMessage("Work order saved successfully.", "success");

        selectedTests = [];
        renderSelectedTests();
        loadTests();
    } catch (error) {
        showMessage(error.message, "danger");
    }
}

function displayResult(order) {
    const result = document.getElementById("result");
    const body = document.getElementById("resultBody");

    body.innerHTML = `
        <div class="row">
            <div class="col-md-6">
                <p><strong>WOE Number:</strong> ${escapeHtml(order.woeNumber)}</p>
                <p><strong>Client:</strong> ${escapeHtml(order.clientName)}</p>
                <p><strong>Patient Code:</strong> ${escapeHtml(order.patientCode)}</p>
                <p><strong>Patient:</strong> ${escapeHtml(order.patientName)}</p>
            </div>
            <div class="col-md-6">
                <p><strong>Age:</strong> ${order.age}</p>
                <p><strong>Gender:</strong> ${escapeHtml(order.gender)}</p>
                <p><strong>Mobile:</strong> ${escapeHtml(order.mobileNumber)}</p>
                <p><strong>Status:</strong> ${escapeHtml(order.status)}</p>
            </div>
        </div>

        <h5>Tests</h5>
        <table class="table table-sm table-bordered">
            <thead>
                <tr>
                    <th>Test</th>
                    <th>Rate</th>
                    <th>Qty</th>
                    <th>Amount</th>
                </tr>
            </thead>
            <tbody>
                ${order.tests.map(test => `
                    <tr>
                        <td>${escapeHtml(test.testName)}</td>
                        <td>₹${Number(test.rate).toFixed(2)}</td>
                        <td>${test.quantity}</td>
                        <td>₹${Number(test.amount).toFixed(2)}</td>
                    </tr>
                `).join("")}
            </tbody>
        </table>

        <h4 class="text-end">Total: ₹${Number(order.totalAmount).toFixed(2)}</h4>
    `;

    result.classList.remove("d-none");
    result.scrollIntoView({ behavior: "smooth" });
}

function showMessage(message, type) {
    document.getElementById("message").innerHTML =
        `<div class="alert alert-${type}">${escapeHtml(message)}</div>`;
}

function clearMessage() {
    document.getElementById("message").innerHTML = "";
}

function escapeHtml(value) {
    return String(value)
        .replaceAll("&", "&amp;")
        .replaceAll("<", "&lt;")
        .replaceAll(">", "&gt;")
        .replaceAll('"', "&quot;")
        .replaceAll("'", "&#039;");
}

function escapeJs(value) {
    return String(value).replaceAll("\\", "\\\\").replaceAll("'", "\\'");
}
