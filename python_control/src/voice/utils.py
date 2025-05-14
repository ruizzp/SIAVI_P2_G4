import socket

def send_to_unity(message):
    try:
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect(("localhost", 5005))
        s.sendall(message.encode("utf-8"))
        s.close()
        print(f"➡️ Enviado para Unity: {message}")
    except Exception as e:
        print("Erro ao enviar para Unity:", e)