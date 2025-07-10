function handleCardKeydown(event, url) {
    if (event.key === "Enter" || event.key === " " || event.code === "Space") {
        event.preventDefault();
        window.location.href = url;
    }
}


// function addRoomType() {
//     var select = document.getElementById('roomTypeSelect');
//     var value = select.value;
//     if (!value) return;
//     // Prevent duplicates
//     if ([...document.querySelectorAll('#roomTypeChips .chip')].some(c => c.textContent.trim().startsWith(value))) return;
//     // Add chip
//     var chip = document.createElement('span');
//     chip.className = 'chip';
//     chip.innerHTML = value + '<button type="button" class="btn-close ms-1" aria-label="Remove" onclick="removeRoomType(\'' + value + '\')"></button>';
//     document.getElementById('roomTypeChips').appendChild(chip);
//     // Add hidden input
//     var input = document.createElement('input');
//     input.type = 'hidden';
//     input.name = 'RoomTypes';
//     input.value = value;
//     document.querySelector('form').appendChild(input);
// }
//
// function removeRoomType(value) {
//     // Remove chip
//     [...document.querySelectorAll('#roomTypeChips .chip')].forEach(c => {
//         if (c.textContent.trim().startsWith(value)) c.remove();
//     });
//     // Remove hidden input
//     [...document.querySelectorAll('input[name="RoomTypes"]')].forEach(i => {
//         if (i.value === value) i.remove();
//     });
// }