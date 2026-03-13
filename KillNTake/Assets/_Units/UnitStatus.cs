using UnityEngine;

public enum Team { Player, Enemy }

public class UnitStatus : MonoBehaviour
{
    [SerializeField] public Team currentTeam;

    public void SetTeam(Team newTeam)
    {
        ApplyTeamColor();
    }

    private void ApplyTeamColor()
    {
    }
}
