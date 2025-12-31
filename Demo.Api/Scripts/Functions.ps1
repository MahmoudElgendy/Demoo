# function Say-Hello{
#     Write-Output "Hello, World!"
# }

# function Say-Hello{
#     param (
#         [string]$Name
#     )
#     Write-Output "Hello, $Name!"
# }

function Say-Hello{
    param (
        [string]$Name = "DefaultName"
    )
    Write-Output "Hello, $Name!"
}

function Add-Numbers {
    param (
        [int]$A,
        [int]$B
    )

    $A + $B
}

function Get-FullName{
    [cmdletbinding()]
    param (
        [string]$FirstName,
        [string]$LastName
    )
    "$FirstName $LastName"
}

function Get-UpperText {
    param (
        [Parameter(ValueFromPipeline)]
        [string]$Text
    )

    process {
        $Text.ToUpper()
    }
}

function Divide-Numbers {
    param (
        [int]$A,
        [int]$B
    )

    try {
        $A / $B
    }
    catch {
        Write-Error "Cannot divide by zero"
    }
}
