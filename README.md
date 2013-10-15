# Simple.MailServer

Simple SMTP Mail Server in C# 5 (using async features)

## Installation

    ```
    PM> Install-Package Simple.MailServer
    ```

## Supported Features

- basic SMTP protocol (extendible through provided interfaces)
- multiple port bindings
- extensively using C# 5 async (for better performance than just threads)
- certain MIME features for encoding, date parsing (RFC 2822, RFC 5335)

## Planned Features

- basic POP3 protocol (extendible through interfaces)
- SSL/TLS support

# Related Projects

If you need to parse mail messages, have a look at MimeKit:  
https://github.com/jstedfast/MimeKit

# Contact

Please let me know if there are bugs or if you have suggestions how to improve the code.

And maybe follow me [@quadfinity](https://twitter.com/quadfinity) :)
