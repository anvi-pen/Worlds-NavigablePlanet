using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HungerDigestionManager : MonoBehaviour
{
    // Stage of hunger, deduced from hunger level, determines color
    // of the UI hunger bar and the speed of the player
    public enum HUNGER_STAGE
    {
        HUNGRY,
        SATISFIED,
        FULL
    }
    private HUNGER_STAGE m_stage;
    public HUNGER_STAGE stage
    {
        get
        {
            return m_stage;
        }
        private set
        {
            if (value == m_stage)
                return;

            m_stage = value;
            sliderFillImage.color = hungerColor[(int)m_stage];
            playerController.walkSpeed = walkSpeeds[(int)m_stage];
        }
    }
    // Colors for UI hunger bar
    private Color[] hungerColor = { Color.red, Color.yellow, Color.green };
    private float[] walkSpeeds;

    // Max stamina that player can have
    [SerializeField] private float maxFull = 100;

    // Player's current stamina / hunger
    private float m_fullMeter;
    private float fullMeter
    {
        get
        {
            return m_fullMeter;
        }
        set
        {
            if (value == m_fullMeter)
                return;

            m_fullMeter = value;
            if (value > maxFull)
                m_fullMeter = maxFull;
            else if (value < 0)
                m_fullMeter = 0;

            if (m_fullMeter == maxFull)
                stage = HUNGER_STAGE.FULL;
            else
                stage = (HUNGER_STAGE) Mathf.FloorToInt((m_fullMeter / maxFull) * 3);

            hungerSlider.value = m_fullMeter;
        }
    }

    // Rate at which stamina decreases depending on if player walks or jumps
    public enum RATE_TYPE
    {
        MOVE,
        JUMP
    }
    [SerializeField] private float moveRate = 2f;
    [SerializeField] private float jumpRate = 5f;

    [SerializeField] Slider hungerSlider;
    Image sliderFillImage;

    // Food near the player that can be eaten
    private List<FoodLogic> foodInRange = new List<FoodLogic>();

    [SerializeField] private PlayerController playerController;

    [SerializeField] private Transform seedParent;

    // Start is called before the first frame update
    void Start()
    {
        // Set player's initial walk speed
        float curWalkSpeed = playerController.walkSpeed;
        walkSpeeds = new float[] { curWalkSpeed / 2, curWalkSpeed * (3f / 4), curWalkSpeed };

        // Get image component of UI hunger slider to later adjust according to player's stamina later in the game
        sliderFillImage = hungerSlider.transform.Find("Fill Area").Find("Fill").GetComponent<Image>();

        // Player initially has full stamina
        stage = HUNGER_STAGE.FULL;
        fullMeter = maxFull;

        // Initalize UI hunger slider
        hungerSlider.maxValue = maxFull;
        hungerSlider.minValue = 0;
        hungerSlider.value = fullMeter;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Decrease player stamina based on player movement / jumping
    public void IncreaseHunger(RATE_TYPE type)
    {
        float rate;
        switch (type)
        {
            case RATE_TYPE.MOVE:
                rate = moveRate * Time.deltaTime;
                break;
            case RATE_TYPE.JUMP:
                rate = jumpRate;
                break;
            default:
                rate = 0;
                break;
        }

        fullMeter -= rate;
    }

    // Called when the player presses the key to eat a nearby fruit
    private void OnEat(InputValue eatValue)
    {
        if (eatValue.Get<float>() > 0)
        {
            if (foodInRange.Count > 0)
            {
                // Remove fruit from list of fruits near the player that can be eaten
                int foodToEatIndex = foodInRange.Count - 1;
                FoodLogic foodToEat = foodInRange[foodToEatIndex];
                foodInRange.RemoveAt(foodToEatIndex);
                // Get stamina points from the fruit
                fullMeter += foodToEat.GetNutritionPoints();
                // Eat fruit and poop seed shortly after
                StartCoroutine(DigestAndPoop(foodToEat.gameObject));
            }
        }
    }

    // If within a certain range, add fruit to list of fruits that can currently be
    // eaten by the player
    private void OnTriggerEnter(Collider other)
    {
        FoodLogic food = other.gameObject.GetComponent<FoodLogic>();
        if ((food != null) && food.active)
            foodInRange.Add(food);
    }

    // If outside of a certain range, remove fruit from list of fruits that can currently be
    // eaten by the player
    private void OnTriggerExit(Collider other)
    {
        FoodLogic food = other.gameObject.GetComponent<FoodLogic>();
        if (food != null)
            foodInRange.Remove(food);
    }

    // Destroys the fruit and instantiates a seed near the player after a few seconds
    IEnumerator DigestAndPoop(GameObject food)
    {
        food.GetComponent<FoodLogic>().active = false;
        food.SetActive(false);
        food.transform.parent.gameObject.SetActive(false);

        yield return new WaitForSeconds(5);

        GameObject seed = food.GetComponent<FoodLogic>().seed;

        Instantiate(seed, transform.position, transform.rotation, seedParent);
        Destroy(food.transform.parent.gameObject);

        //food.transform.parent.position = transform.position;
        //food.transform.parent.rotation = transform.rotation;

        //food.transform.parent.gameObject.SetActive(true);
        //food.SetActive(true);
        //food.GetComponent<FoodLogic>().active = true;
    }
}
