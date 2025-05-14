import mediapipe as mp
import cv2
from gesture.utils import send_to_unity
import json

def detect_gestures(on_gesture):
    mp_hands = mp.solutions.hands
    hands = mp_hands.Hands(min_detection_confidence=0.7)
    cap = cv2.VideoCapture(0)

    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        image = cv2.flip(frame, 1)
        rgb = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
        results = hands.process(rgb)

        gesture_type = "none"
        norm_x, norm_y = 1000, 1000  # valor padrão no centro da tela

        if results.multi_hand_landmarks:
            for hand in results.multi_hand_landmarks:
                thumb_tip = hand.landmark[4]
                index_tip = hand.landmark[8]
                palm = hand.landmark[0]  # ponto base da mão

                dx = abs(thumb_tip.x - index_tip.x)
                dy = abs(thumb_tip.y - index_tip.y)

                if dx < 0.05 and dy < 0.05:
                    gesture_type = "pinça"
                else:
                    gesture_type = "mão_aberta"

                norm_x, norm_y = palm.x, palm.y  # posição normalizada da mão
                break  # só usa a primeira mão detetada

        # Enviar para Unity
        payload = {
            "gesture": gesture_type,
            "x": round(norm_x, 3),
            "y": round(norm_y, 3)
        }
        send_to_unity(json.dumps(payload))
        ##print(payload)
        on_gesture(gesture_type)

        # Mostrar na tela (debug)
        cv2.putText(image, f"{gesture_type}", (10, 40),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 200, 255), 2)
        cv2.imshow("Gestos", image)

        if cv2.waitKey(5) & 0xFF == 27:
            break

    cap.release()
    cv2.destroyAllWindows()