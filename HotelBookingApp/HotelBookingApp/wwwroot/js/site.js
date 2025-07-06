// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    const chatInput = document.getElementById("chatInput");
    const chatSubmit = document.getElementById("chatSubmit");
    const chatResponse = document.getElementById("chatResponse");

    if (chatInput && chatSubmit && chatResponse) {
        chatSubmit.addEventListener("click", async function () {
            const message = chatInput.value.trim();
            if (!message) return;

            try {
                const response = await fetch('/api/chatbot/message', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({ text: message })
                });

                const data = await response.json();
                chatResponse.innerText = data.text;
                chatResponse.style.display = 'block';
            } catch (error) {
                chatResponse.innerText = "Something went wrong. Please try again.";
                chatResponse.style.display = 'block';
                console.error(error);
            }
        });
    }
});


// Show popup on FAB click
document.getElementById('chatFab').onclick = function() {
    document.getElementById('chatPopup').style.display = 'block';
};
// Hide popup on close
document.getElementById('closeChatPopup').onclick = function() {
    document.getElementById('chatPopup').style.display = 'none';
};


function handleCardKeydown(event, url) {
    if (event.key === "Enter" || event.key === " " || event.code === "Space") {
        event.preventDefault();
        window.location.href = url;
    }
}


function addRoomType() {
    var select = document.getElementById('roomTypeSelect');
    var value = select.value;
    if (!value) return;
    // Prevent duplicates
    if ([...document.querySelectorAll('#roomTypeChips .chip')].some(c => c.textContent.trim().startsWith(value))) return;
    // Add chip
    var chip = document.createElement('span');
    chip.className = 'chip';
    chip.innerHTML = value + '<button type="button" class="btn-close ms-1" aria-label="Remove" onclick="removeRoomType(\'' + value + '\')"></button>';
    document.getElementById('roomTypeChips').appendChild(chip);
    // Add hidden input
    var input = document.createElement('input');
    input.type = 'hidden';
    input.name = 'RoomTypes';
    input.value = value;
    document.querySelector('form').appendChild(input);
}

function removeRoomType(value) {
    // Remove chip
    [...document.querySelectorAll('#roomTypeChips .chip')].forEach(c => {
        if (c.textContent.trim().startsWith(value)) c.remove();
    });
    // Remove hidden input
    [...document.querySelectorAll('input[name="RoomTypes"]')].forEach(i => {
        if (i.value === value) i.remove();
    });
}