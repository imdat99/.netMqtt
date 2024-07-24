// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const client = mqtt.connect(messageServer);
client.on('connect', function () {
    console.log('Connected to broker');
    client.subscribe('test/#', { qos: 1 }, function (err) {
        if (!err) {
            console.log('Subscribed to chat room');
        } 
    });
});

client.on('message', function (topic, message) {
    const chatMessage = message.toString();
    console.log('Received message:', chatMessage);
    
    const chatDiv = document.getElementById('message');
    const messageElement = document.createElement('p');
    messageElement.textContent = chatMessage;
    chatDiv.appendChild(messageElement);
});