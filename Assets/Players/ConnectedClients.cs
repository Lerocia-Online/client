namespace Players {
  using System.Collections.Generic;

  public static class ConnectedClients {
    public static User MyUser;
    public static Player MyPlayer;
    public static Dictionary<int, Player> Players = new Dictionary<int, Player>();
  }
}
