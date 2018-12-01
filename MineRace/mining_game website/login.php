<?php

    session_start();

    if (isset($_SESSION['user_id'])) {
        header("Location: index.php");
    }

    $server = '';
    $username = '';
    $password = '';
    $database = '';

    try {
        $conn = new PDO("mysql:host=$server;dbname=$database;", $username, $password);
    } catch (PDOException $e) {
        die("Connection Failed: " . $e.getMessage());
    }

    if (isset($_POST['submit']) && !empty($_POST['submit'])):

        $message = '';

        if (empty($_POST['username']) || empty($_POST['password'])):
            $message = 'Please fill in all fields';
        endif;

        if (empty($message)):
            $records = $conn->prepare("SELECT id,uid,pwd FROM users WHERE uid = :uid");
            $records->bindParam(":uid", $_POST['username']);
            $records->execute();
            $results = $records->fetch(PDO::FETCH_ASSOC);
            if (count($results) > 0):
                if (password_verify($_POST['password'], $results['pwd'])):
                    $_SESSION['user_id'] = $results['id'];
                    header("Location: index.php");
                endif;
            else:
                $message = 'No user with name ' . $_POST['username'];
            endif;
        endif;
    endif;
?>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>Login - UA Studios</title>
    <link rel="stylesheet" href="style.css">
    <script src="https://use.fontawesome.com/32e8da75e3.js"></script>
    <style>body, html { font-family: "6809Chargen", "Courier New", sans-serif; color: orange; }</style>
</head>

<body>
    <div class="login-container">
        <div class="header-image-home">
            <a href="index.php"><i class="fa fa-home" aria-hidden="true" style="color:white; font-size: 40px; float:left; margin-left: 5px; margin-top: 5px;"></i></a>
            <div class="icon">
                <img id="user-image-circle" src="resources/defaultAvatar.png">
            </div>
        </div>

        <h4 style="font-size: 18px; margin-top: 0px;">Sign in to your account</h4>
        <form method="POST" action="login">
            <div class="form-input">
                <input type="text" name="username" placeholder="username">
            </div>
            <div class="form-input">
                <input type="password" name="password" placeholder="password">
            </div>

            <input type="submit" name="submit" value="LOGIN" class="submitbtn"><br>
            <a href="resetpassword" id="forgotpassword">Forgot Password?</a>
            <a href="register" id="register">Create an account</a>
        </form>
        <?php if(!empty($message)): ?>
        <h4 style="color: #e82929; font-weight: bold; font-style: italic; margin-top: 40px;"><?= $message ?></h4>
        <?php endif; ?>
    </div>
</body>
</html>
