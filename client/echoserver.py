import socket

def start_echo_server(host: str = '127.0.0.1', port: int = 9696):
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server_socket:
        server_socket.bind((host, port))
        server_socket.listen(1)
        print(f"Server listening on {host}:{port}")
        
        while True:
            client_socket, addr = server_socket.accept()
            with client_socket:
                print(f"Connected by {addr}")
                while True:
                    data = client_socket.recv(1024)
                    print(f"Data: {data}")
                    if not data:
                        break
                    client_socket.sendall(data)

if __name__ == "__main__":
    start_echo_server()
