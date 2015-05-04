# RDS Factor

Two-factor authentication for Remote Desktop Services (RDS).

RDS Factor consist of two components:
* A server component that talks RADIUS with RD Web and the RD Gateway.
* An updated version of the RD Web pages that interacts with the RADIUS server and an opt-in multi-factor form.

RDS Factor works by sending an SMS to users -- after they authenticate with username / password -- which when entered allow them entrance to RD Web. When users click on an applicaiton in RD Web a window is opened in the gateway for that user. In that way, users that are not authenticated via RD Web can't get access through RD Gateway. Compare this to the standard RDS setup, where there is no way to share state between RD Web and RD Gateway, meaning that the gateway is always open for user / password only authentication.

It is also possible to disable two factor authentication in RDS Factor. In this case, RDS Factor still maintains state between RD Web and RD Gateway, ensuring that users have logged into RD Web before connections are allowed through the gateway. That means custom multi-factor authenticators can be put in front of RD Web and also protect the gateway.

Tested on Windows 2012 R2.

## Prerequisites

An RDS setup. There are many options for orchestrating the RDS setup; the minimal RDS setup for use with RDS Factor consist of two servers: 
* Active Directory; and
* RDS with Gateway component enabled

Use 'Active Directory Users and Computers' to add a mobile number to relevant LDAP users in the Active Directory. The tools is not installed pr. default. It's located under `Add Roles -> Features -> Remote Server Administration Tools -> AD DS Tools -> AD DS Snap-Ins And Command-Line Tools`

## Installation

The core RADIUS server is included in this project as a git submodule. Fetch it by:

```
C:\RDSFactor> git submodule update --init
```

### RD Web update
RDS factor comes with a customized version of the RD Web pages. To install these run:

```
C:\RDSFactor> install-web.bat
```

After install go and configure the application in IIS. `RDWeb -> Pages -> Application Settings`. You should configure the following settings:
* `RadiusServer` IP of the radius server
* `RadiusSecret` Shared secret -- of your own chosing -- used for encrypting RADIUS traffic

### RADIUS server installation

The RADIUS server component can be installed on any server reacheable by both the RD Web and the RD Gateway. To install the server as a service run:

```
C:\RDSFactor> install-server.bat
```

After install go and configure the server. Open the file `RDSFactor/server/bin/release/conf.ini` for editing. You should configure the following settings:
* `LDAPDomain` IP of LDAP server to authenticate user and lookup phonenumber against 
* `ADField` LDAP attribute to use for looking up the user's phonenumber
* `EnableOTP` Boolean (0|1) that indicates whether to use the 2. factor for auth
* `Debug` Enable debug output to  `RDSFactor/server/bin/release/log.txt`
* `{client}={shared secret}` IP of RD Web and shared secret -- same as `RadiusSecret`-- for encryption
* `Provider` URL of SMS provider. RDS Factor inserts the number and a message in the two variable, `***NUMBER***` and `***TEXTMESSAGE***`, in the provider URL. An example URL using the SMS gateway cpsms: https://www.cpsms.dk/sms/?username=myuser&password=mypassword&recipient=***NUMBER***&message=***TEXTMESSAGE***&from=CPSMS

To reload the configuration restart the RADIUS server service by running
```
C:\RDSFactor> restart-server.bat
```

## Logging

Log output from the RADAR client in RD Web is output into:
```
C:\RDSFactor\web\RDWeb\Pages\log> Radius_Debug.txt
```

Log output from the RADAR server:
```
C:\RDSFactor\server\bin\Release> log.txt
```

## Acknowledgements

* Claus Isager - for the first Open Source two factor RDS authenticator; which this project is based upon.
* Nikolay Semov - for the core RADIUS server 

## License

RDS Factor is an open source project, sponsored by [Origo Systems A/S](https://origo.io), under the GNU general public license version 3.
