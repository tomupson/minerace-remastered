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
            <li><a href="index.php" id="currentPage">HOME</a></li>
            <li><a href="downloads.php">DOWNLOADS</a></li>
            <li><a href="help.php">HELP</a></li>
            <?php if(!isset($_SESSION['user_id'])): ?>
            <li><a href="register.php">REGISTER</a></li>
            <li><a href="login.php">LOGIN</a></li>
            <?php endif; ?>
        </ul></nav>
    </div>
    <div class="mainBody">
        <p id="faq_subheading">So what is this game?</p>
        <p id="faq_answer">Mining Game is a multiplayer, time-constrained game in which the goal is to collect as many resources as you can before the timer runs out. The map is filled with resources that spawn in the map randomly every game. Rarer materials spawn at lower levels, and every material is worth a certain amount of points, so you can choose whether to rush to the bottom and get all the diamond goodies or stay around the top and win by pure quantity of the more common resources.</p>
        <p id="faq_subheading">Where did the idea come from?</p>
        <p id="faq_answer">This game was created for my Computer Science coursework from 2017-2018 and the idea was inspired from an entry into the Video Game Coding competition 'Ludum Dare'. This game was called "Diggin'" and was created by <a href="https://www.youtube.com/brackeys" target="_blank">Brackeys</a> and can be found <a href="http://bit.ly/2c8T56d" target="_blank">here</a></p>
        <p id="faq_subheading">Where can I download the game?</p>
        <p id="faq_answer">You can download the game on the "Downloads" page or by clicking <a href="downloads.php">here</a></p>
    </div>
</body>
</html>
