# MailAutoconf

MailAutoconf detects settings for POP3, IMAP and SMTP servers starting with an email address. It searches various data sources as described in [Mail Autoconfig](https://benbucksch.github.io/autoconfig-spec/draft-ietf-mailmaint-autoconfig.html) or probes known email service network ports.

### Example

##### Detect settings for john.doe@yahoo.com

```c#
var serverSettings = new ServerSettingsFinder(mailAddress: "john.doe@yahoo.com")
	.GetSettings()
	.SelectBest();

Console.WriteLine($"POP3: {serverSettings.Pop3}");
Console.WriteLine($"IMAP: {serverSettings.Imap}");
Console.WriteLine($"SMTP: {serverSettings.Smtp}");
```

##### Output at console

```
POP3: pop.mail.yahoo.com, 995, SslTls, User: john.doe@yahoo.com, OAuth2, password-cleartext, Verification: Unverified
IMAP: imap.mail.yahoo.com, 993, SslTls, User: john.doe@yahoo.com, OAuth2, password-cleartext, Verification: Unverified
SMTP: smtp.mail.yahoo.com, 465, SslTls, User: john.doe@yahoo.com, OAuth2, password-cleartext, Verification: Unverified
```

### MailAutoconf on GitHub

For more information see https://github.com/ThomasContrib/MailAutoconf.
