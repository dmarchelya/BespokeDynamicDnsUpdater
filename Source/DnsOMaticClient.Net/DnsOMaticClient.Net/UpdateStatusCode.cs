using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace DnsOMaticClient.Net
{
	public enum UpdateStatusCode
	{
		[NativeUpdateStatusCode("good")]
		[Description("The update was scheduled successfully")]
		Good,

		[NativeUpdateStatusCode("nochg")]
		[Description("Indicates success, with no change. DNS-O-Matic will not re-distribute successive nochg updates. This is to avoid abuse as well as save bandwidth and server infrastructure for both DNS-O-Matic and the supported services.")]
		NoChange,

		[NativeUpdateStatusCode("badauth")]
		[Description("The DNS-O-Matic username or password specified are incorrect. No updates will be distributed to services until this is resolved.")]
		BadAuth,

		[NativeUpdateStatusCode("notfqdn")]
		[Description("The hostname specified is not a fully-qualified domain name. If no hostnames included, notfqdn will be returned once.")]
		NotFullyQualifiedDomainName,

		[NativeUpdateStatusCode("nohost")]
		[Description("The hostname passed could not be matched to any services configured. The service field will be blank in the return code.")]
		NoHost,

		[NativeUpdateStatusCode("numhost")]
		[Description("You may update up to 20 hosts. numhost is returned if you try to update more than 20 or update a round-robin.")]
		NumberOfHosts,

		[NativeUpdateStatusCode("abuse")]
		[Description("The hostname is blocked for update abuse.")]
		Abuse,

		[NativeUpdateStatusCode("badagent")]
		[Description("The user-agent is blocked.")]
		BadAgent,

		[NativeUpdateStatusCode("dnserr")]
		[Description("DNS error encountered. Stop updating for 30 minutes, and optionally ask the user to contact DNS-O-Matic.")]
		DnsErrror,

		[NativeUpdateStatusCode("911")]
		[Description("There is a problem or scheduled maintenance on DNS-O-Matic. Stop updating for 30 minutes, and optionally ask the user to contact DNS-O-Matic.")]
		DnsOMaticError,

		Unknown
	}
}
