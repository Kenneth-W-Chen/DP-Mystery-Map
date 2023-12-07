using System.Collections;
using PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerFollow : GameplayScript
{
    public GameObject playerObject;

    private static PlayerFollow _reference;

    private float _zPosition;

    protected override void Start()
    {
        if (_reference is not null)
        {
            Destroy(this.gameObject);
            return;
        }

        base.Start();

        _reference = this;
        _zPosition = transform.position.z;
        StartCoroutine(LateStart());
    }

    // LateStart is called one frame after Start finishes execution
    private IEnumerator LateStart()
    {
        yield return null;
        if (playerObject)
        {
            yield break;
        }

        while (PlayerController.playerControllerReference is null)
            yield return null;
        this.playerObject = PlayerController.playerControllerReference.gameObject;
    }


    // LateUpdate is called after all Update methods are finished
    void LateUpdate()
    {
        var newPosition = playerObject.transform.position;
        this.transform.position = new Vector3(newPosition.x, newPosition.y, _zPosition);
    }

    private void OnDestroy()
    {
        if (_reference == this)
            _reference = null;
        SceneManager.sceneLoaded -= OnLevelLoad;
    }
}