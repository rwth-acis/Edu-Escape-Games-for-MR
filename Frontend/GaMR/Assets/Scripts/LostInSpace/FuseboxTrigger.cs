using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class is only for the fusebox game object. It manages, places and destroys
 * the fuses. It checks whether the current fuse is the right and blocks it.
 */
public class FuseboxTrigger : MonoBehaviour {

    public string acceptingFuseTag;

    public AudioSource currentSource;
    public AudioSource fuseBreak;

    private Vector3 fusePosition;
    private Quaternion fuseRotation;
    private GameObject currentFuse;
    private GameObject oldFuse;

    private float lastPlaced = 0f;

	/**
     * At the beginning set the broken fuse as current fuse and save its position
     */
	void Start () {
        currentFuse = GameObject.FindGameObjectWithTag("FuseBroken");
        fusePosition = currentFuse.transform.position;
        fuseRotation = currentFuse.transform.rotation;
        oldFuse = null;
	}
	
	/**
     * Move the current fuse to the right place and delete the old fuse.
     */
	void Update () {
		if (!currentFuse.transform.position.Equals(fusePosition) && !currentFuse.transform.rotation.Equals(fuseRotation)) {
            currentFuse.transform.position = Vector3.Lerp(currentFuse.transform.position, fusePosition, Time.deltaTime);
            currentFuse.transform.rotation = Quaternion.Lerp(currentFuse.transform.rotation, fuseRotation, Time.deltaTime);
        }

        if (oldFuse != null) {
            Destroy(oldFuse);
            oldFuse = null;
        }
	}

    /**
     * Is called when a game object (with rigid body) enters the collider. Check
     * for the tag, identify the fuse and update it.
     */
    private void OnTriggerEnter(Collider collider) {
        GameObject gameObject = collider.gameObject;
        Debug.Log("Was trigger by " + gameObject.tag);

        if (QuestManager.GetInstance().IsFuseFixed()) {
            Debug.Log("The quest is already solved");
            return;
        }

        if (lastPlaced != 0f && Time.time - lastPlaced < 60) {
            Debug.Log("The fuse is overheating... Waited: " + (Time.time - lastPlaced));
            return;
        }
        lastPlaced = Time.time;

        switch (gameObject.tag) {
            case "Fuse10":
                UpdateCurrentFuse(gameObject);
                break;
            case "Fuse15":
                UpdateCurrentFuse(gameObject);
                break;
            case "Fuse20":
                UpdateCurrentFuse(gameObject);
                break;
            case "FuseBroken":
                UpdateCurrentFuse(gameObject);
                break;
            default:
                Debug.Log("You can't put that here");
                break;
        }
    }

    /**
     * Stops grabbing of the fuse and updates it. If it is the right fuse
     * Accept() it and Decline() if not.
     */
    private void UpdateCurrentFuse(GameObject gameObject) {
        if (currentFuse.Equals(gameObject)) {   // Do not update if it is the same fuse
            Debug.Log("It's the same fuse! Not changing it.");
            return;
        }

        gameObject.GetComponent<GrabController>().StopGrabbing();       // Stop grabbing
        gameObject.GetComponent<GrabController>().SetMoveable(false);   // Disable moveability
        oldFuse = currentFuse;
        currentFuse = gameObject;

        if (acceptingFuseTag.Equals(currentFuse.tag)) {
            Accept();
        } else {
            StartCoroutine(Decline());
        }
    }

    private void Accept() {
        QuestManager.GetInstance().FuseFixed();

        // Auditive effect
        currentSource.Play();

    }

    private IEnumerator Decline() {
        fuseBreak.Play();
        yield return new WaitForSeconds(4);
        startParticles();
        // Visual and/or auditive effect for the wrong fuse. Block the fuse for some minutes...
    }

    private IEnumerator stopParticles() {
        yield return new WaitForSeconds(56);
        gameObject.GetComponentInParent<ParticleSystem>().emissionRate = 0;
    }

    private void startParticles() {
        gameObject.GetComponentInParent<ParticleSystem>().emissionRate = 100;
        StartCoroutine(stopParticles());
    }
}
