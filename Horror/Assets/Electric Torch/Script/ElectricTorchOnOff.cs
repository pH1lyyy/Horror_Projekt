using UnityEngine;
using UnityEngine.UI;
public class ElectricTorchOnOff : MonoBehaviour
{
    EmissionMaterialGlassTorchFadeOut _emissionMaterialFade;

    public enum LightChoose
    {
        noBattery,
        withBattery
    }

    public LightChoose modoLightChoose;
    public string onOffLightKey = "F";
    private KeyCode _kCode;
    public Text cooldownText;
    public bool _PowerPickUp = false;
    public float intensityLight = 2.5F;
    [SerializeField] float _lightTime = 0.05f;

    private bool _flashLightOn = false;

    private float _cooldownTimer = 0f;
    private float _activeTimer = 0f;
    private bool _isInCooldown = false;

    private readonly float _cooldownDuration = 20f;
    private readonly float _activeDuration = 5f;
    private PlayerPickup playerPickup;

    public AudioClip flashlightToggleSound; 
    private AudioSource _audioSource;


    private void Awake()
    {
        playerPickup = FindObjectOfType<PlayerPickup>();
    }

    private bool IsHoldingFlashlight()
    {
        if (playerPickup == null) return false;

        GameObject heldItem = playerPickup.GetCurrentItem();
        return heldItem != null && heldItem.CompareTag("Flashlight");
    }

    void Start()
    {
        GameObject _scriptControllerEmissionFade = GameObject.Find("default");

        if (_scriptControllerEmissionFade != null)
        {
            _emissionMaterialFade = _scriptControllerEmissionFade.GetComponent<EmissionMaterialGlassTorchFadeOut>();
        }
        else
        {
            Debug.Log("Cannot find 'EmissionMaterialGlassTorchFadeOut' script");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        _kCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), onOffLightKey);
    }


    void Update()
    {
        if (System.Enum.TryParse(onOffLightKey, out _kCode))
        {
            _kCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), onOffLightKey);
        }

        if (_flashLightOn)
        {
            _activeTimer += Time.deltaTime;

            if (_activeTimer >= _activeDuration)
            {
                TurnOffFlashlight();
                StartCooldown();
            }
        }

        if (_isInCooldown)
        {
            _cooldownTimer += Time.deltaTime;
            float remaining = Mathf.Ceil(_cooldownDuration - _cooldownTimer);
            cooldownText.text = $"Latarka przegrzana! Pozostało {remaining} s";
            cooldownText.enabled = true;

            if (_cooldownTimer >= _cooldownDuration)
            {
                _isInCooldown = false;
            }
        }
        else
        {
            cooldownText.enabled = false;
        }

        if (IsHoldingFlashlight())
        {
            switch (modoLightChoose)
            {
                case LightChoose.noBattery:
                    NoBatteryLight();
                    break;
                case LightChoose.withBattery:
                    WithBatteryLight();
                    break;
            }
        }
        else
        {
            if (_flashLightOn)
            {
                TurnOffFlashlight();
            }
        }

    }

    void InputKey()
    {
        if (Input.GetKeyDown(_kCode))
        {
            if (_flashLightOn)
            {
                TurnOffFlashlight();
            }
            else if (!_isInCooldown)
            {
                TurnOnFlashlight();
            }
        }
    }

    void TurnOnFlashlight()
    {
        _flashLightOn = true;
        _activeTimer = 0f;
        PlayToggleSound();
    }

    void TurnOffFlashlight()
    {
        _flashLightOn = false;
        GetComponent<Light>().intensity = 0.0f;
        _emissionMaterialFade.OffEmission();
        PlayToggleSound();
    }

    void PlayToggleSound()
    {
        if (flashlightToggleSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(flashlightToggleSound);
        }
    }


    void StartCooldown()
    {
        _isInCooldown = true;
        _cooldownTimer = 0f;
    }

    void NoBatteryLight()
    {
        if (_flashLightOn)
        {
            GetComponent<Light>().intensity = intensityLight;
            _emissionMaterialFade.OnEmission();
        }
        else
        {
            GetComponent<Light>().intensity = 0.0f;
            _emissionMaterialFade.OffEmission();
        }

        InputKey();
    }

    void WithBatteryLight()
    {
        if (_flashLightOn)
        {
            GetComponent<Light>().intensity = intensityLight;
            intensityLight -= Time.deltaTime * _lightTime;
            _emissionMaterialFade.TimeEmission(_lightTime);

            if (intensityLight < 0)
            {
                intensityLight = 0;
            }

         
        }
        else
        {
            GetComponent<Light>().intensity = 0.0f;
            _emissionMaterialFade.OffEmission();

       
        }

        InputKey();
    }
    private RaycastHit hit;
    private float monsterHitTime = 0f;
    private float requiredFocusTime = 2f;

    void LateUpdate()
    {
        if (_flashLightOn)
        {
            Transform cameraTransform = Camera.main.transform;
            transform.position = cameraTransform.position;
            transform.rotation = cameraTransform.rotation;

            if (Physics.Raycast(transform.position, transform.forward, out hit, 15f))
            {
                if (hit.collider.CompareTag("Monster"))
                {
                    monsterHitTime += Time.deltaTime;

                    if (monsterHitTime >= requiredFocusTime)
                    {
                        MonsterAI monsterAI = hit.collider.GetComponent<MonsterAI>();
                        if (monsterAI != null)
                        {
                            monsterAI.ApplyStun(5f);
                            monsterHitTime = 0f;
                        }
                    }
                }
                else
                {
                    monsterHitTime = 0f;
                }
            }
            else
            {
                monsterHitTime = 0f;
            }
        }
        else
        {
            monsterHitTime = 0f;
        }
    }


}
