# RDS Factor

Two-factor authentication for Remote Desktop Services (RDS).

RDS Factor consist of two components:
* A server component that talks RADIUS with RD Web and the RD Gateway
* An updated version of the RD Web pages that interacts with the RADIUS server and ask users to enter one-time passwords sent to their phone before letting them in.

Tested on Windows 2012 R2.

## Prerequisites

An RDS setup. There are many options for orchestrating the RDS setup; the minimal RDS setup for use with RDS Factor consist of two servers: 
* Active Directory; and
* RDS with Gateway component enabled

## Installation

### RD Web update
RDS factor comes with a customized version of the RD Web pages. To install these run:

```
$ install-web.bat
```

After install go and configure the application in IIS. RDWeb -> Pages -> Application Settings. You should configure the following settings:
* RadiusServer (IP of the radius server)
* RadiusSecret (Shared secret used for encryption of RADIUS traffic)

### RADIUS server installation

The RADIUS server component can be installed on any server reacheable by both the RD Web and the RD Gateway. To install the server as a service run:

```
$ install-server.bat
```

After install go and configure the server. Open the file RDSFactor/server/bin/release/conf.ini for editing. You should configure the following settings:
* LDAPDomain (IP of server to authenticate the user against and lookup phonenumber)
* ADField (LDAP attribute to use for looking the user's phonenumber)
* {client}={shared secret} should be added in the clients section 

Note that the client should be the IP of RD Web, and the shared secret must match the value of RadiusSecret in the IIS.  

## Acknowledgements

* Claus Isager - for the proof of concept two factor RDS authentication 
* Nikolay Semov - for the core RADIUS server 
