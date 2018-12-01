<?php

    session_start();

    $server = '';
    $username = '';
    $password = '';
    $database = '';

    try {
        $conn = new PDO("mysql:host=$server;dbname=$database;", $username, $password);
    } catch (PDOException $e) {
        die("Connection Failed: " . $e.getMessage());
    }

    if (isset($_SESSION['user_id'])) {
        $records = $conn->prepare("SELECT id,uid,email,pwd,xp,hscore FROM users WHERE id = :id");
        $records->bindParam(":id", $_SESSION['user_id']);
        $records->execute();
        $results = $records->fetch(PDO::FETCH_ASSOC);
        $user = NULL;

        if (count($results) > 0) {
            $user = $results;
        }
    }

?>

<!DOCTYPE html>
<html>
<head>
    <title>MINING GAME</title>
    <link rel="stylesheet" href="style.css">
    <script src="functions.js"></script>
</head>
<body>
    <div class="header">
        <h1 id="title_text">MINING GAME</h1>
        <nav><ul>
            <li><a href="index.php">HOME</a></li>
            <li><a href="downloads.php">DOWNLOADS</a></li>
            <li><a href="help.php" id="currentPage">HELP</a></li>
            <li><a href="register.php">REGISTER</a></li>
            <li><a href="login.php">LOGIN</a></li>
        </ul></nav>
    </div>
    <div class="mainBody">

    </div>
</body>
</html>
