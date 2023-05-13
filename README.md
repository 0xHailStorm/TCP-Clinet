# TCP-Clinet

This is a C# console application that demonstrates how to perform a HTTPS POST request to a server using a TcpClient and SslStream, and then read the response.

# Requirements
  * Visual Studio 2019 or higher.
  * .NET Framework 4.7.2 or higher.

# How to use
* 1- Clone or download the repository to your local machine.
* 2- Open the solution file in Visual Studio.
* 3- Build the solution.
* 4- Run the program.
  
When the program is run, it performs a HTTPS POST request to "i.instagram.com" and reads the response. The response is then printed to the console.

# Code Structure

The application consists of a single C# file Program.cs. The file contains the following functions:

* Main: Entry point of the application. It sends an HTTPS POST request to the Instagram API to check the availability of a username.
* ParseRequest: Takes in HTTP method, host, path, headers, cookies, and post data as parameters and returns an HTTP request string.
* Set_Client: Creates a TCP client and connects to a given server and port. If proxy is enabled, it sends a CONNECT request to the proxy server to establish a tunnel.
* WriteSSl: Creates an SSL stream and authenticates with the server. It then sends a packet of data over the SSL stream.
* ValidateServerCertificate: A callback function used to validate the server certificate during SSL authentication.
* ReadResponse: A function that reads the response from the SSL stream and prints it to the console.
* ResetBuffer: A function that clears the buffer used for reading the SSL stream.
* Response: A class that holds the TCP client connection and SSL stream for a response.

