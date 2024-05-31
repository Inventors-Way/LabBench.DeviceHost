import socket
import logging

def send_and_receive(message: str, timeout: int = 10) -> str:
    """
    Sends a text string to a socket on port 9797 and returns the response.
    
    Args:
        host (str): The host to connect to.
        message (str): The message to send.
        port (int): The port to connect to (default is 9797).
        timeout (int): The timeout for the socket in seconds (default is 1).
    
    Returns:
        str: The response from the server.
    
    Raises:
        Exception: If any socket error or timeout occurs.
    """
    response = ''
    
    # Set up logging
    logging.basicConfig(level=logging.INFO)
    logger = logging.getLogger(__name__)
    
    try:
        # Create a socket
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
            # Set the timeout for the socket
            s.settimeout(1)
            
            # Connect to the server
            logger.info(f"Connecting to 127.0.0.1:9797")
            s.connect(('127.0.0.1', 9797))
            
            # Send the message
            logger.info(f"Sending message: {message}")
            s.sendall(message.encode('utf-8'))
            
            # Receive the response
            logger.info("Waiting for response")
            response = s.recv(4096).decode('utf-8')
            logger.info(f"Received response: {response}")
    
    except socket.timeout:
        logger.error("Socket timed out")
        raise Exception("Socket timed out")
    
    except socket.error as e:
        logger.error(f"Socket error occurred: {e}")
        raise Exception(f"Socket error occurred: {e}")
    
    except Exception as e:
        logger.error(f"An unexpected error occurred: {e}")
        raise Exception(f"An unexpected error occurred: {e}")
    
    return response

# Example usage:
response = send_and_receive('example.com', 'Hello, World!')
print(response)

def main():
    message = 'Hello, Local Server!'
    
    try:
        response = send_and_receive(message)
        print(f"Response from server: {response}")
    except Exception as e:
        print(f"An error occurred: {e}")

if __name__ == "__main__":
    main()

