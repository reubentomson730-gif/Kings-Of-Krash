using UnityEngine;
using UnityEngine.InputSystem;
public class vehicleLogic : MonoBehaviour
{
    [SerializeField] Transform[] springPoints;
    [SerializeField] Transform[] steerPoints;
    [SerializeField] GameObject[] tyres;
    [SerializeField] GameObject[] steerTyres;
    [SerializeField] float restLength;
    [SerializeField] float springStrength;
    [SerializeField] float damperStiffness;
    [SerializeField] float acceleration;
    [SerializeField] float maxSteerAngle;
    [SerializeField] float maxVisualSteerAngle;
    [SerializeField] float tireGripFactor;
    [SerializeField]Rigidbody vehicleRb;
    float forwardInput = 1f;
    float steerInput = 0.5f;

    void Start()
    {
        steerInput = 0f;   
       
    }

    // Update is called once per frame
    void Update()
    {
        Steering();
        Visuals();
    }
    void FixedUpdate() 
    {
        Suspension();
        
    }
    void Visuals() 
    {
        foreach (GameObject steerTyre in steerTyres) 
        {
            float i = steerInput * maxVisualSteerAngle;
            //steerTyre.transform.Rotate(Vector3.up, acceleration *forwardInput* Time.deltaTime, Space.Self);
            steerTyre.transform.localEulerAngles = new Vector3(steerTyre.transform.localEulerAngles.x, i, steerTyre.transform.localEulerAngles.z);
        }
        foreach (GameObject tyre in tyres)
        {
            tyre.transform.Rotate(Vector3.up, acceleration * forwardInput * Time.deltaTime, Space.Self);
        }

    }
    void Steering() 
    {
        if (Keyboard.current.dKey.isPressed)
        {
            foreach (Transform steerPoint in steerPoints) 
            {
                steerInput = 1f;
                steerPoint.localRotation = Quaternion.Euler(0f, steerInput * maxSteerAngle, 0f);
            }
            /*steerInput = 1f;
            vehicleRb.AddTorque(maxSteerAngle * steerInput * transform.up, ForceMode.Acceleration);*/
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            steerInput = -1f;   
            /*steerInput = -1f;
            vehicleRb.AddTorque(maxSteerAngle * steerInput * transform.up, ForceMode.Acceleration);*/
        }
        else 
        {
            steerInput = 0f;
        }
        foreach (Transform steerPoint in steerPoints)
        {
            
            steerPoint.localRotation = Quaternion.Euler(0f, steerInput * maxSteerAngle, 0f);
        }
    }
    void Suspension() 
    {
        foreach (Transform springPoint in springPoints) 
        {
            RaycastHit hit;
            Vector3 forwardDir = springPoint.forward;
            Vector3 steerDir = springPoint.right;
            if (Physics.Raycast(springPoint.position,-transform.up,out hit, restLength)) 
            {
                
                float displacement = restLength - hit.distance;
                float springVelocity = Vector3.Dot(springPoint.up,vehicleRb.GetPointVelocity(springPoint.position));
                float netForce = (displacement * springStrength) - (springVelocity * damperStiffness);
                vehicleRb.AddForceAtPosition(netForce * springPoint.up, springPoint.position);


                vehicleRb.AddForceAtPosition(forwardDir * acceleration * forwardInput, springPoint.position);
                Vector3 tireVelocity = vehicleRb.GetPointVelocity(springPoint.position);
                
                float steerVal = Vector3.Dot(steerDir, tireVelocity);
                float intendedValChange = -steerVal * tireGripFactor * vehicleRb.mass;
                //float finalChange = intendedValChange / Time.deltaTime;
                vehicleRb.AddForceAtPosition(steerDir* intendedValChange, springPoint.position);
            }

            
        }
    }
    
}
