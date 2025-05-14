import socket

def listen_for_language_change():
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    s.bind(("localhost", 5050))
    s.listen(5)
    print("🌐 Aguardando comandos de idioma (pt/en)...")

    while True:
        conn, _ = s.accept()
        data = conn.recv(1024).decode("utf-8").strip().lower()
        conn.close()
        if data in ["pt", "en"]:
            yield data