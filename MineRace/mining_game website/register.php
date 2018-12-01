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

        $user_handler = $conn->prepare("SELECT uid FROM users WHERE uid = :username");
        $user_handler->bindParam(":username", $_POST['username']);
        $user_handler->execute();

        if ($user_handler->rowCount() > 0):
            $message = 'Username already taken. Please try another';
        endif;

        $email_handler = $conn->prepare("SELECT email FROM users WHERE email = :email");
        $email_handler->bindParam(":email", $_POST['email']);
        $email_handler->execute();
        if ($email_handler->rowCount() > 0):
            $message = 'Email is already in use. Please try another.';
        elseif (!filter_var($_POST['email'], FILTER_VALIDATE_EMAIL)):
            $message = 'Please enter a valid email address';
        endif;

        if (empty($_POST['username']) || empty($_POST['email']) || empty($_POST['password']) || empty($_POST['confirmpassword'])):
            $message = 'Please fill in all required fields';
        endif;

        if ($_POST['password'] != $_POST['confirmpassword']):
            $message = 'Passwords do not match. Please try again';
        endif;

        if (empty($message)):
            $sql = "INSERT INTO users (uid, email, pwd, xp, hscore) VALUES (:username, :email, :password, 0, 0)";
            $stmt = $conn->prepare($sql);

            $stmt->bindParam(":username", $_POST['username']);
            $stmt->bindParam(":email", $_POST['email']);
            $stmt->bindParam(":password", password_hash($_POST['password'], PASSWORD_BCRYPT));

            if ($stmt->execute()):
                $records = $conn->prepare("SELECT id,uid,pwd FROM users WHERE username = :username");
                $records->bindParam(":username", $_POST['username']);
                $records->execute();
                $results = $records->fetch(PDO::FETCH_ASSOC);
                $_SESSION['user_id'] = $results['id'];
                header("Location: index.php");
            else:
                $message = 'Unexpected Error occured. Please try again.';
            endif;
        endif;
    endif;
?>

<!DOCTYPE html>
<html>
<head>
    <meta charset="UTF-8">
    <title>Register - UA Studios</title>
    <link rel="stylesheet" href="style.css">
    <script src="https://use.fontawesome.com/32e8da75e3.js"></script>
    <style>body, html { font-family: "6809Chargen", "Courier New", sans-serif; color: orange; }</style>
</head>

<body>
    <div class="register-container">
        <div class="header-image-home">
            <a href="index.php"><i class="fa fa-home" aria-hidden="true" style="color:white; font-size: 40px; float:left; margin-left: 5px; margin-top: 5px;"></i></a>
            <div class="icon">
                <img id="user-image-circle" src="resources/defaultAvatar.png">
            </div>
        </div>

        <h4 style="font-size: 18px; margin-top: 0px;">Register for an account</h4>
        <form method="POST" action="register">
            <div class="form-input">
                <input type="text" name="username" placeholder="username">
            </div>
            <div class="form-input">
                <input type="email" name="email" placeholder="email address">
            </div>
            <div class="form-input">
                <input type="password" name="password" placeholder="password">
            </div>
            <div class="form-input">
                <input type="password" name="confirmpassword" placeholder="confirm password">
            </div>

            <input type="submit" name="submit" value="REGISTER" class="submitbtn"><br>
            <h4 style="text-align: center; margin-left: -40px; margin-top: 0px;">Already have an account? <a href="login" id="login">Login</a></h4>
            <?php if (!empty($message)): ?>
            <h4 style="color: #e82929; font-weight: bold; font-style: italic;"><?= $message ?></h4>
            <?php endif; ?>
        </form>
    </div>
</body>

</html>
