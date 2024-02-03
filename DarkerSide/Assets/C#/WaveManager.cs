using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public int maxWaves = 3;
    public float waveDuration = 21f;
    //public string nextSceneName = "NextScene";

    
    private int currentWave = 1;
    
    public Text countdownText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartWaves());    
    }
    IEnumerator StartWaves()
    {
        while (currentWave <= maxWaves)
        {
            countdownText.color = Color.white;

            countdownText.text = "Wave " + currentWave ; // I must delete or change that text after 2 second
            yield return new WaitForSeconds(2);
            countdownText.text = "Battle Started!!!";
            yield return new WaitForSeconds(2);
            countdownText.text = "";

            StartWave();

            float timer = waveDuration;

            while(timer>0f)
            {
                yield return new WaitForSeconds(0);

                countdownText.text = "" + Mathf.Floor(timer);
                
                timer -= Time.deltaTime;
                if(timer<=5f)
                {
                    countdownText.color = Color.red;
                }
                else
                {
                    countdownText.color = Color.white;
                }
            }

            ComplateWave();
        }
        Debug.Log("All waves complated");
        
       // LoadNextScene();
    }

    void StartWave()
    {
        waveDuration = waveDuration+(currentWave-1)*5; // add more 5 second for the next wave
       // Debug.Log("Wave " + currentWave + " started!");
        
        

    }
    void ComplateWave()
    {
        currentWave++;
        // Debug.Log("Wave " + currentWave + " ended!");
        if (currentWave <= maxWaves)
        {
            ChoosePover();
        }
        

    }

    void ChoosePover()
    {
        Debug.Log("ChoosePover");
    }


}
