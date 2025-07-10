function filterTable() {
    const startDate = document.getElementById('tableStartDate').value;
    const endDate = document.getElementById('tableEndDate').value;
    const minRevenue = parseFloat(document.getElementById('tableMinRevenue').value) || 0;
    const rows = document.querySelectorAll('table tbody tr');
    rows.forEach(row => {
        const date = row.children[0].innerText;
        const revenue = parseFloat(row.children[3].innerText.replace(/[^0-9.-]+/g,"")) || 0;
        let show = true;
        if (startDate && new Date(date) < new Date(startDate)) show = false;
        if (endDate && new Date(date) > new Date(endDate)) show = false;
        if (minRevenue && revenue < minRevenue) show = false;
        row.style.display = show ? '' : 'none';
    });
}
function clearFilter() {
    document.getElementById('tableStartDate').value = '';
    document.getElementById('tableEndDate').value = '';
    document.getElementById('tableMinRevenue').value = '';
    filterTable();
}