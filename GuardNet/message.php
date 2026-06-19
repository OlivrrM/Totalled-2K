<?php
$filepath = "message.txt";

// Handle POST request to update the message
if ($_SERVER['REQUEST_METHOD'] === 'POST') {
    $message = $_POST['message'] ?? '';
    file_put_contents($filepath, $message);
    echo "OK";
    exit;
}

// Handle GET request to return the message
if (file_exists($filepath)) {
    echo file_get_contents($filepath);
} else {
    echo "";
}
?>