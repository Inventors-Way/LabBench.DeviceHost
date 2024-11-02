import socket
import logging


class CPARDevice:
   def __init__(self, address:str = '127.0.0.1', port:int = 9797, timeout:int = 10):
      logging.basicConfig(level=logging.INFO)
      self.logger = logging.getLogger(__name__)
      self.address = address
      self.port = port
      self.timeout = timeout

   def execute(self, message: str, timeout: int = 10) -> str:
      response = ''
     
      try:
         # Create a socket
         with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
               s.settimeout(self.timeout)
               s.connect((self.address, self.port))
               s.sendall(message.encode('utf-8'))
               response = s.recv(4096).decode('utf-8')
      
      except socket.timeout:
         self.logger.error("Socket timed out")
         raise Exception("Socket timed out")
      
      except socket.error as e:
         self.logger.error(f"Socket error occurred: {e}")
         raise Exception(f"Socket error occurred: {e}")
      
      except Exception as e:
         self.logger.error(f"An unexpected error occurred: {e}")
         raise Exception(f"An unexpected error occurred: {e}")
      
      return response


if __name__ == '__main__':
   device = CPARDevice()

   print('Hello, World!')