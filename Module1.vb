Imports System.Management.Automation
Imports System.Management.Automation.Runspaces
Imports Microsoft.ComputeCluster.CCPPSH

Module Module1

    Sub Main()
        Dim iss As InitialSessionState = InitialSessionState.CreateDefault()
        Dim warning As PSSnapInException = Nothing
        'You have to import a SnapIn like this
        iss.ImportPSSnapIn("Microsoft.HPC", warning)
        If (warning IsNot Nothing) Then
            Console.WriteLine(warning)
            Return
        End If

        'Create a Runspace, open it and set it to the Runspace property of a PowerShell object
        Dim rs As Runspace = RunspaceFactory.CreateRunspace(iss)
        rs.Open()
        Dim ps As PowerShell = PowerShell.Create()
        ps.Runspace = rs

        'Don't forget to AddStatement() before each AddCommand() that you don't want to pipe.
        ps.AddStatement().AddCommand("Get-HpcNode")

        Dim res = ps.Invoke()
        For Each e In res
            'e is an PSObject, which wraps the real object, say HpcNode

            'It prints the wrapped object's type. So you can tell the type in this way.
            'Console.WriteLine(e)

            'HpcNode, together with other HPC types in PowerShell, is defined in ccppsh.dll, in %CCP_HOME%\bin
            'In order to use these types, you need to
            '   1) Add ccppsh.dll as a reference of your project
            '   2) Imports Microsoft.ComputeCluster.CCPPSH
            Dim node As HpcNode = e.BaseObject

            'When VisualStudio tells you some assembly reference is required, just add the reference to your project.
            'For example, in the following statement, it said "Microsoft.Ccp.ClusterModel"(for node.NodeState) is required.
            'It's an assembly of HPC, in %CCP_HOME%\bin. So you copy the dll from the Head Node to your local dev machine and
            'add it as an reference of your project.
            'NOTE:
            '   1) You 're likely to find all your dependencies in %CCP_HOME%\bin of your Head Node.
            '   2) An easy way to run your app is to drop your app in that folder and run from there.
            Console.WriteLine($"{node.InstanceName}{vbTab}{node.HealthState}{vbTab}{node.NodeState}{vbTab}{node.Groups}")
        Next

    End Sub

End Module
