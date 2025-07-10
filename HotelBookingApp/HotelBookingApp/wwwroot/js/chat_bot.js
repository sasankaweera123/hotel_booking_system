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