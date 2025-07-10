const roomTypesSelect = document.getElementById('RoomTypes');
const chipsContainer = document.getElementById('roomTypeChips');
const roomTypeCounts = {};
const selectedRoomDetails = [];

function showRoomTypeSelector() {
    roomTypesSelect.style.display = 'block';
    roomTypesSelect.focus();
}

roomTypesSelect.addEventListener('change', async function () {
    for (const option of Array.from(roomTypesSelect.selectedOptions)) {
        if (option.value) {
            const roomType = option.value;

            const room = await getAvailableRoomId(roomType);
            if (!room) {
                alert(`No available room for type: ${roomType}`);
                continue;
            }

            selectedRoomDetails.push({ id: room.id, roomType: room.type });

            if (!roomTypeCounts[roomType]) {
                roomTypeCounts[roomType] = 1;
            } else {
                roomTypeCounts[roomType]++;
            }
        }
    }

    updateChips();
    updateSelectedRoomTypes();
    roomTypesSelect.selectedIndex = -1;
    roomTypesSelect.style.display = 'none';
});


function removeChip(value) {
    if (roomTypeCounts[value]) {
        roomTypeCounts[value]--;
        if (roomTypeCounts[value] <= 0) {
            delete roomTypeCounts[value];
        }

        // Remove the last matching room
        const index = selectedRoomDetails.findLastIndex(r => r.roomType === value);
        if (index !== -1) selectedRoomDetails.splice(index, 1);

        updateChips();
        updateSelectedRoomTypes();
    }
}


function updateChips() {
    chipsContainer.innerHTML = '';
    Object.keys(roomTypeCounts).forEach(value => {
        const option = Array.from(roomTypesSelect.options).find(opt => opt.value === value);
        if (option) {
            const chip = document.createElement('span');
            chip.className = 'badge bg-primary me-1';
            chip.id = 'chip-' + value;
            chip.innerHTML = `${option.text}*${roomTypeCounts[value]} <span style="cursor:pointer;" onclick="removeChip('${value}')">&times;</span>`;
            chipsContainer.appendChild(chip);
        }
    });
}

function updateSelectedRoomTypes() {
    const form = document.querySelector('form');
    Array.from(document.querySelectorAll('input[name^="RoomTypes"]')).forEach(el => el.remove());

    selectedRoomDetails.forEach((room, index) => {
        const inputId = document.createElement('input');
        inputId.type = 'hidden';
        inputId.name = `RoomTypes[${index}].Id`;
        inputId.value = room.id;
        form.appendChild(inputId);

        const inputType = document.createElement('input');
        inputType.type = 'hidden';
        inputType.name = `RoomTypes[${index}].RoomType`;
        inputType.value = room.roomType;
        form.appendChild(inputType);
    });
}


async function showAvailableRooms(roomType) {
    const response = await fetch(`/Room/GetAvailableRooms?roomType=${roomType}`);
    if (response.ok) {
        const res = await response.text();
        const availableRooms = JSON.parse(res);
        document.getElementById('roomType-' + roomType).innerText = `${roomType}: ${availableRooms.length}`;
    } else {
        document.getElementById('roomType-' + roomType).innerText = `${roomType}: 0`;
    }
}

async function getAvailableRoomId(roomType) {
    const response = await fetch(`/Room/GetAvailableRooms?roomType=${roomType}`);
    if (response.ok) {
        const availableRooms = await response.json();
        const selectedIds = selectedRoomDetails.map(r => r.id);
        const unused = availableRooms.find(r => !selectedIds.includes(r.id));
        return unused || null;
    }
    return null;
}


document.addEventListener('DOMContentLoaded', function () {
    const roomTypes = window.availableRoomTypes || [];
    roomTypes.forEach(rt => showAvailableRooms(rt));
})
