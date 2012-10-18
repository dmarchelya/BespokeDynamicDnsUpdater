<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
            xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
            xmlns:msxsl="urn:schemas-microsoft-com:xslt"
            exclude-result-prefixes="msxsl"
            xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
            xmlns:my="my:my">

	<xsl:output method="xml" indent="yes" />

	<xsl:strip-space elements="*" />

	<xsl:template match="/">
		<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi" xmlns:util="http://schemas.microsoft.com/wix/UtilExtension">
			<Fragment>
				<ComponentGroup Id="BespokeDynamicDnsUpdaterWindowsServiceGroup">
					<Component Id='BespokeDynamicDnsUpdaterWindowsService' Directory='INSTALLDIR' Guid='bc52be8f-af0c-4512-bf2b-527794daf3d8' Feature='DefaultFeature'>

						<File Id='BespokeDynamicDnsUpdaterWindowsServiceExeFile' Name='Bespoke.DynamicDnsUpdater.WindowsService.exe' Source='..\Bespoke.DynamicDnsUpdater.WindowsService\bin\$(var.Configuration)\Bespoke.DynamicDnsUpdater.WindowsService.exe' />

						<ServiceInstall Id='BespokeDynamicDnsUpdaterWindowsServiceInstaller' DisplayName='Bespoke Dynamic DNS Updater Service' Name='BespokeDynamicDnsUpdaterService'
						   Description="An Open Source Dynamic DNS Updater Client that runs on a specified interval and updates dynamic DNS hostnames if necessary." Start='auto' Type='ownProcess' ErrorControl='normal'>

							<util:ServiceConfig
								FirstFailureActionType="restart"
								SecondFailureActionType="restart"
								ThirdFailureActionType="restart"
								RestartServiceDelayInSeconds="300"
								ResetPeriodInDays="1"
											/>
							
						</ServiceInstall>

						<ServiceControl Id='BespokeDynamicDnsUpdaterWindowsServiceControl' Name='BespokeDynamicDnsUpdaterService'
						  Stop='uninstall' Remove='uninstall' />
						
						<xsl:apply-templates />
					</Component>
				</ComponentGroup>
			</Fragment>
		</Wix>
	</xsl:template>

	<xsl:template match="wix:File[contains(@Source, '.dll')]|@*"  >
		<xsl:copy>
			<xsl:apply-templates select="node()|@*" />
			<xsl:attribute name="KeyPath">
				<xsl:value-of select="'no'" />
			</xsl:attribute>
			<xsl:attribute name="Source">
				<xsl:value-of select="concat('..\Bespoke.DynamicDnsUpdater.WindowsService\bin\$(var.Configuration)\',substring(@Source,11))" />
			</xsl:attribute>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="wix:File[contains(@Source, '.config')]|@*"  >
		<xsl:copy>
			<xsl:apply-templates select="node()|@*" />
			<xsl:attribute name="KeyPath">
				<xsl:value-of select="'no'" />
			</xsl:attribute>
			<xsl:attribute name="Source">
				<xsl:value-of select="concat('..\Bespoke.DynamicDnsUpdater.WindowsService\bin\$(var.Configuration)\',substring(@Source,11))" />
			</xsl:attribute>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="wix:File[contains(@Source, '.xml')]|@*"  >
		<xsl:copy>
			<xsl:apply-templates select="node()|@*" />
			<xsl:attribute name="KeyPath">
				<xsl:value-of select="'no'" />
			</xsl:attribute>
			<xsl:attribute name="Source">
				<xsl:value-of select="concat('..\Bespoke.DynamicDnsUpdater.WindowsService\bin\$(var.Configuration)\',substring(@Source,11))" />
			</xsl:attribute>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>