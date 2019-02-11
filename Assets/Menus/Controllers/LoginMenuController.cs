namespace Menus.Controllers {
	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using Networking;
	using Networking.Constants;
	using Players;

	public class LoginMenuController : MonoBehaviour {

		private Text _errorText;
		private InputField _usernameInput;
		private InputField _passwordInput;

		private void Awake() {
			transform.Find("LoginButton").GetComponent<Button>().onClick.AddListener(Login);
			_errorText = transform.Find("ErrorText").GetComponent<Text>();
			_usernameInput = transform.Find("UsernameInput").GetComponent<InputField>();
			_passwordInput = transform.Find("PasswordInput").GetComponent<InputField>();
		}
		
		private void Login() {
			if (!NetworkSettings.IsLoggingIn) {
				NetworkSettings.IsLoggingIn = true;
				_errorText.text = "Logging in...";
				StartCoroutine("RequestLogin");
			}
		}
		
		private IEnumerator RequestLogin() {
			string username = _usernameInput.text;
			string password = _passwordInput.text;

			WWWForm form = new WWWForm();
			form.AddField("username", username);
			form.AddField("password", password);

			WWW w = new WWW(NetworkConstants.Api + EndpointConstants.Login, form);
			yield return w;

			if (string.IsNullOrEmpty(w.error)) {
				User user = JsonUtility.FromJson<User>(w.text);
				if (user.success) {
					if (user.error != "") {
						_errorText.text = user.error;
					} else {
						_errorText.text = "Login successful";
						ConnectedClients.MyUser = user;
						NetworkSettings.InitializeNetworkTransport();
					}
				} else {
					_errorText.text = user.error;
					NetworkSettings.IsLoggingIn = false;
				}
			} else {
				_errorText.text = w.error;
				NetworkSettings.IsLoggingIn = false;
			}
		}
	}

}