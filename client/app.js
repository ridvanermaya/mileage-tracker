function CreateUser()
{
    let addEmailSpan = $(`[name="addEmail"]`);
    let addPassowrdSpan = $(`[name="addPassword"]`);
    let confirmPasswordSpan = $(`[name="confirmPassword"]`);
    let inputEmail = $(`#add-email`);
    let inputPassword = $(`#add-password`);
    let inputConfirmPassword = $(`#confirm-passowrd`);
    let notConfirmed = true;

    while(notConfirmed) {
        addEmailSpan.empty();
        addPassowrdSpan.empty();
        confirmPasswordSpan.empty();

        if (inputEmail.val() === "") {
            addEmailSpan.append("*Please enter an email address");
        }
    
        if (inputEmail.val() !== "") {
            if(!(ValidateEmail(inputEmail.val()))) {
                addEmailSpan.append("Not a valid email address");
            }
        }
    
        if (inputPassword.val() === "") {
            addPassowrdSpan.append("Please enter password");
        }
    
        if (!(ComparePasswords(inputPassword.val(), inputConfirmPassword.val()))) {
            confirmPasswordSpan.append("Passwords don't match");
        }
    }
    

    const user =
    {
        Email: $(`#add-email`).val(),
        Password: $(`#add-passowrd`).val()
    };
}

function ComparePasswords(passwordOne, passwordTwo) {
    if (passwordOne === passwordTwo) {
        return true;
    } else {
        return false;
    }
}

function ValidateEmail(email) 
{
 if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(email))
  {
    return true;
  }
    return false;
}