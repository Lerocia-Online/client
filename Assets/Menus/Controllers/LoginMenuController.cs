namespace Menus.Controllers {
	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using UnityEngine.SceneManagement;
	using Networking;
	using Networking.Constants;
	using Lerocia.Characters;
	using Lerocia.Characters.Players;

	public class LoginMenuController : MonoBehaviour {

		private Text _loginErrorText;
		private InputField _loginUsernameInput;
		private InputField _loginPasswordInput;
		
		private Text _registerErrorText;
		private InputField _registerUsernameInput;
		private InputField _registerPasswordInput;
		private InputField _registerRepeatPasswordInput;

		private void Awake() {
			transform.Find("LoginButton").GetComponent<Button>().onClick.AddListener(Login);
			_loginErrorText = transform.Find("LoginErrorText").GetComponent<Text>();
			_loginUsernameInput = transform.Find("LoginUsernameInput").GetComponent<InputField>();
			_loginPasswordInput = transform.Find("LoginPasswordInput").GetComponent<InputField>();
			
			transform.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(Register);
			_registerErrorText = transform.Find("RegisterErrorText").GetComponent<Text>();
			_registerUsernameInput = transform.Find("RegisterUsernameInput").GetComponent<InputField>();
			_registerPasswordInput = transform.Find("RegisterPasswordInput").GetComponent<InputField>();
			_registerRepeatPasswordInput = transform.Find("RegisterRepeatPasswordInput").GetComponent<InputField>();
		}
		
		private void Login() {
			if (!NetworkSettings.IsLoggingIn) {
				NetworkSettings.IsLoggingIn = true;
				_loginErrorText.text = "Logging in...";
				StartCoroutine("RequestLogin");
			}
		}
		
		private IEnumerator RequestLogin() {
			string username = _loginUsernameInput.text;
			string password = _loginPasswordInput.text;

			WWWForm form = new WWWForm();
			form.AddField("username", username);
			form.AddField("password", password);

			WWW w = new WWW(NetworkConstants.Api + EndpointConstants.Login, form);
			yield return w;

			if (string.IsNullOrEmpty(w.error)) {
				DatabasePlayer databasePlayer = JsonUtility.FromJson<DatabasePlayer>(w.text);
				if (databasePlayer.success) {
					if (databasePlayer.error != "") {
						_loginErrorText.text = databasePlayer.error;
					} else {
						_loginErrorText.text = "Login successful";
						ConnectedCharacters.MyDatabasePlayer = databasePlayer;
						NetworkSettings.InitializeNetworkTransport();
						SceneManager.LoadScene("Lerocia");
					}
				} else {
					_loginErrorText.text = databasePlayer.error;
					NetworkSettings.IsLoggingIn = false;
				}
			} else {
				_loginErrorText.text = w.error;
				NetworkSettings.IsLoggingIn = false;
			}
		}

		private void Register() {
			if (!NetworkSettings.IsRegistering) {
				NetworkSettings.IsRegistering = true;
				_registerErrorText.text = "Registering...";
				StartCoroutine("RequestRegister");
			}
		}
		
		private IEnumerator RequestRegister() {
			string username = _registerUsernameInput.text;
			string password = _registerPasswordInput.text;
			string repeatPassword = _registerRepeatPasswordInput.text;

			if (password != repeatPassword) {
				_registerErrorText.text = "Passwords do not match.";
				NetworkSettings.IsRegistering = false;
				yield break;
			}

			WWWForm form = new WWWForm();
			form.AddField("username", username);
			form.AddField("password", password);
			form.AddField("origin_x", 0);
			form.AddField("origin_y", 0);
			form.AddField("origin_z", 0);

			WWW w = new WWW(NetworkConstants.Api + EndpointConstants.Register, form);
			yield return w;

			if (string.IsNullOrEmpty(w.error)) {
				DatabasePlayer databasePlayer = JsonUtility.FromJson<DatabasePlayer>(w.text);
				if (databasePlayer.success) {
					if (databasePlayer.error != "") {
						_registerErrorText.text = databasePlayer.error;
					} else {
						_registerErrorText.text = "Register successful";
						NetworkSettings.IsRegistering = false;
					}
				} else {
					_registerErrorText.text = databasePlayer.error;
					NetworkSettings.IsRegistering = false;
				}
			} else {
				_registerErrorText.text = w.error;
				NetworkSettings.IsRegistering = false;
			}
		}
	}

}