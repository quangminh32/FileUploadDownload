"use strict";

console.log("Initializing Admin SignalR connection...");

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/signalr/chatHub")
    .withAutomaticReconnect()
    .build();

var currentReplyUser = null;
var activeUsers = new Set();

// Disable reply button until connection is established
document.getElementById("sendReplyButton").disabled = true;
document.getElementById("sendImageButton").disabled = true;

// Handle user messages
connection.on("ReceiveMessage", function (user, message, imageUrl) {
    console.log("Admin received message from user:", user, message, imageUrl);
    
    // Add user to active users if not already there
    if (!activeUsers.has(user)) {
        activeUsers.add(user);
        updateActiveUsersList();
    }
    
    // Add message to chat
    var li = document.createElement("li");
    li.className = "mb-2 p-2 border rounded";
    
    var messageHtml = `
        <strong class="text-primary">${user}</strong> 
        <small class="text-muted">${new Date().toLocaleTimeString()}</small>
        <br>
        <span>${message}</span>
        <button class="btn btn-sm btn-outline-primary float-end" onclick="setReplyUser('${user}')">Reply</button>
    `;
    
    // Thêm ảnh nếu có
    if (imageUrl && imageUrl.trim() !== "") {
        messageHtml += `
            <br>
            <img src="${imageUrl}" class="img-fluid mt-2" style="max-width: 200px; max-height: 200px; border-radius: 8px;" alt="User Image" />
        `;
    }
    
    li.innerHTML = messageHtml;
    document.getElementById("messagesList").appendChild(li);
    
    // Auto-scroll to bottom
    var messagesContainer = document.querySelector('.card-body');
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
});

// Handle admin replies to specific users
connection.on("ReceiveAdminReply", function (adminName, message, imageUrl) {
    console.log("Admin reply received:", adminName, message, imageUrl);
    
    var li = document.createElement("li");
    li.className = "mb-2 p-2 border rounded bg-light";
    
    var messageHtml = `
        <strong class="text-success">${adminName} (Admin)</strong> 
        <small class="text-muted">${new Date().toLocaleTimeString()}</small>
        <br>
        <span>${message}</span>
    `;
    
    // Thêm ảnh nếu có
    if (imageUrl && imageUrl.trim() !== "") {
        messageHtml += `
            <br>
            <img src="${imageUrl}" class="img-fluid mt-2" style="max-width: 200px; max-height: 200px; border-radius: 8px;" alt="Admin Image" />
        `;
    }
    
    li.innerHTML = messageHtml;
    document.getElementById("messagesList").appendChild(li);
    
    // Auto-scroll to bottom
    var messagesContainer = document.querySelector('.card-body');
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
});

// Handle user disconnection
connection.on("UserDisconnected", function (user) {
    console.log("User disconnected:", user);
    activeUsers.delete(user);
    updateActiveUsersList();
});

// Connection events
connection.onreconnecting(function () {
    console.log("Admin SignalR: Reconnecting...");
    document.getElementById("sendReplyButton").disabled = true;
    document.getElementById("sendImageButton").disabled = true;
});

connection.onreconnected(function () {
    console.log("Admin SignalR: Reconnected!");
    document.getElementById("sendReplyButton").disabled = false;
    document.getElementById("sendImageButton").disabled = false;
});

connection.onclose(function () {
    console.log("Admin SignalR: Connection closed");
    document.getElementById("sendReplyButton").disabled = true;
    document.getElementById("sendImageButton").disabled = true;
});

// Start connection
console.log("Starting Admin SignalR connection...");
connection.start().then(function () {
    console.log("Admin SignalR connection established successfully!");
    document.getElementById("sendReplyButton").disabled = false;
    document.getElementById("sendImageButton").disabled = false;
}).catch(function (err) {
    console.error("Admin SignalR connection failed:", err.toString());
    document.getElementById("sendReplyButton").disabled = true;
    document.getElementById("sendImageButton").disabled = true;
});

// Set reply user
window.setReplyUser = function(user) {
    currentReplyUser = user;
    document.getElementById("replyToUser").value = user;
    document.getElementById("adminMessageInput").focus();
    document.getElementById("sendReplyButton").disabled = false;
    document.getElementById("sendImageButton").disabled = false;
};

// Update active users list
function updateActiveUsersList() {
    var usersList = document.getElementById("activeUsersList");
    usersList.innerHTML = "";
    
    activeUsers.forEach(function(user) {
        var li = document.createElement("li");
        li.className = "mb-1";
        li.innerHTML = `
            <span class="badge bg-success">${user}</span>
            <button class="btn btn-sm btn-outline-primary ms-2" onclick="setReplyUser('${user}')">Reply</button>
        `;
        usersList.appendChild(li);
    });
}

// Send reply button event
document.getElementById("sendReplyButton").addEventListener("click", function (event) {
    if (!currentReplyUser) {
        alert("Please select a user to reply to!");
        return;
    }
    
    var message = document.getElementById("adminMessageInput").value.trim();
    if (!message) {
        alert("Please enter a message!");
        return;
    }
    
    console.log("Admin sending reply to:", currentReplyUser, message);
    
    // Send reply to specific user
    connection.invoke("SendAdminReply", currentReplyUser, "Admin", message).catch(function (err) {
        console.error("Error sending admin reply:", err.toString());
        alert("Error sending reply: " + err.toString());
    });
    
    // Clear input
    document.getElementById("adminMessageInput").value = "";
    event.preventDefault();
});

// Send image button event
document.getElementById("sendImageButton").addEventListener("click", function (event) {
    if (!currentReplyUser) {
        alert("Please select a user to send image to!");
        return;
    }
    
    var imageInput = document.getElementById("adminImageInput");
    var message = document.getElementById("adminMessageInput").value.trim();
    
    if (!imageInput.files || imageInput.files.length === 0) {
        alert("Please select an image to send!");
        return;
    }
    
    var file = imageInput.files[0];
    console.log("Admin sending image to:", currentReplyUser, file.name);
    
    // Tạo FormData
    var formData = new FormData();
    formData.append("file", file);
    
    // Upload ảnh
    fetch("/api/Image/Upload", {
        method: "POST",
        body: formData
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            console.log("Image uploaded successfully:", data);
            
            // Gửi reply kèm ảnh đến user cụ thể
            connection.invoke("SendAdminReplyWithImage", currentReplyUser, "Admin", message || "", data.imageUrl).catch(function (err) {
                console.error("Error sending admin reply with image:", err.toString());
                alert("Error sending reply with image: " + err.toString());
            });
            
            // Clear inputs
            imageInput.value = "";
            document.getElementById("adminMessageInput").value = "";
        } else {
            alert("Error uploading image: " + data.message);
        }
    })
    .catch(error => {
        console.error("Error uploading image:", error);
        alert("Error uploading image: " + error.toString());
    });
    
    event.preventDefault();
});

// Enter key to send reply
document.getElementById("adminMessageInput").addEventListener("keypress", function(event) {
    if (event.key === "Enter" && !event.shiftKey) {
        event.preventDefault();
        document.getElementById("sendReplyButton").click();
    }
}); 