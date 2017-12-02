function Send-Datagram {
    [CmdletBinding()]
    param(
      [parameter(Mandatory=$true,Position=0, ValueFromPipeline=$true,ValueFromPipelineByPropertyName=$true)][string] $data,
      [parameter(Mandatory=$false)][string] $address="127.0.0.1",
      [parameter(Mandatory=$false)][int] $port=8125
    )

    $ipAddress = $null
    $parseResult = [System.Net.IPAddress]::TryParse($address, [ref] $ipAddress)

    if ( $parseResult -eq $false )
    {
        $addresses = [System.Net.Dns]::GetHostAddresses($address)

        if ( $addresses -eq $null )
        {
            throw "Unable to resolve address: $address"
        }

        $ipAddress = $addresses[0]
    }

    $endpoint = New-Object System.Net.IPEndPoint($ipAddress, $port)
    $udpClient = New-Object System.Net.Sockets.UdpClient

    $encodedData=[System.Text.Encoding]::ASCII.GetBytes($data)
    $bytesSent=$udpClient.Send($encodedData,$encodedData.length,$endpoint)

    $udpClient.Close()
}