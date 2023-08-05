using UnityEngine; 
public class CameraControl : MonoBehaviour
 { 
    public float m_DampTime = 0.2f; //Tiempo de espera para mover la cámara
    public float m_ScreenEdgeBuffer = 4f; //Pequeño padding para que los tanques no se pegen a los bordes 
    public float m_MinSize = 6.5f; //Tamaño mínimo de zoom 
    [HideInInspector]
    public Transform[] m_Targets;

    private Camera m_Camera; //la cámara 
    private float m_ZoomSpeed; //velocidad de zoom 
    private Vector3 m_MoveVelocity; //velocidad de movimiento 
    private Vector3 m_DesiredPosition; //posición a la que quiero llegar


    private void Awake() {
          m_Camera = GetComponentInChildren<Camera>(); 
    }

    private void FixedUpdate() {
         Move(); //Mueve la cámara 
         Zoom(); //ajusta el tamaño de la cámara 
    }


    private void Move() { //Busco la posicón intermedia entre los dos tanques 
    FindAveragePosition(); //Muevo la cámara de forma suave 
    transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime); 
    }

    private void FindAveragePosition() {
         Vector3 averagePos = new Vector3();
         int numTargets = 0;

         for (int i = 0; i < m_Targets.Length; i++) {
            if (!m_Targets[i].gameObject.activeSelf) continue;
            averagePos += m_Targets[i].position;
            numTargets++;
        }
            if (numTargets > 0) 
            averagePos /= numTargets;
            averagePos.y = transform.position.y;
            m_DesiredPosition = averagePos;
    }


    private void Zoom() {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime); 
    }

    private float FindRequiredSize() {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);
        float size = 0f;

        for (int i = 0; i < m_Targets.Length; i++) {
            if (!m_Targets[i].gameObject.activeSelf) continue;
            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);
            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));
            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }

        size += m_ScreenEdgeBuffer;
        size = Mathf.Max(size, m_MinSize);
        return size; 
        }

    public void SetStartPositionAndSize() {
        FindAveragePosition();
        transform.position = m_DesiredPosition;
        m_Camera.orthographicSize = FindRequiredSize();
    }

 }
