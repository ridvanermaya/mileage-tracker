var uri = "https://localhost:5001/api/UserProfile";

function CreateUserProfile(e) {
  $(`[name="addFirstName"]`).empty();
  $(`[name="addLastName"]`).empty();

  const userProfile = {
    FirstName: $(`#add-first-name`).val(),
    LastName: $(`#add-last-name`).val(),
    PhoneNumber: $(`#add-phone-number`).val()
  };

  if (userProfile.FirstName === "") {
    $(`[name="addFirstName"]`).append("* First name is required.");
    return;
  } else if (userProfile.LastName === "") {
    $(`[name="addLastName"]`).append("* Last name is required.");
    return;
  } else {
    $.ajax({
      type: "POST",
      accepts: "application/json",
      url: uri,
      contentType: "application/json",
      data: JSON.stringify(userProfile),
      error: error => {
        console.log(error);
      },
      success: function(result) {
        console.log("Success");
      }
    });
  }
}
