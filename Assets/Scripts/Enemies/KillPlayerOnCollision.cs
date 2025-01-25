using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class KillPlayerOnCollision : MonoBehaviour
{
    private void Update()
    {
        Vector3 playerPos = PlayerController.activePlayer.transform.position;
        float distance = Vector3.Distance(transform.position, playerPos);

        //IT DEPENDS ON CHARACTER CONTROLLERS RADIUS WHICH FOR NOW IS 0.25!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (distance < 0.45f) PlayerController.activePlayer.OnPlayerDeath?.Invoke();
    }
}
