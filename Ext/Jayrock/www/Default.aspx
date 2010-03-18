<%@ Page Language="C#" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
<head>
    <title>Jayrock: JSON and JSON-RPC for .NET</title>
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="ProgId" content="VisualStudio.HTML" />
    <meta name="Originator" content="Microsoft Visual Studio .NET 7.1" />
    <meta name="Description" content="Jayrock is an open source project to bring JSON and JSON-RPC to .NET" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link rel="stylesheet" type="text/css" href="Default.css" />
</head>
<body>
    <div id="Content">
        <div id="Toc">
            <p id="TocTitle">
                Table of Contents</p>
            <ul>
                <li><a href="#what-is">What is Jayrock?</a></li>
                <li><a href="#get-source">Where is the Source, Luke?</a></li>
                <li><a href="#compiling">Compiling</a></li>
                <li><a href="#contributing">Contributing to the Project</a></li>
                <li><a href="#community">Community &amp; Discussions</a></li>
                <li><a href="#setup">Setting Up Jayrock</a></li>
                <li><a href="#quick-start">ASP.NET Quick Start</a></li>
                <li><a href="#samples">Samples &amp; Demos</a></li>
                <li><a href="#docs">Documents</a></li>
                <li><a href="#qa">Questions?</a></li>
            </ul>
            <p>
                Updated:
                <% = DateTime.Now.ToString("dd MMM yyyy") %>
            </p>
        </div>
        <h1 class="h1-first">
            <a id="what-is" name="what-is">What is Jayrock?</a></h1>
        <p><em>Web services, the light and simple way!</em></p>
        <p>
            <a href="http://developer.berlios.de/projects/jayrock/">Jayrock</a> is a modest and an open source 
            (<a href="http://www.opensource.org/licenses/lgpl-license.php"><acronym title="GNU Lesser General Public License">LGPL</acronym></a>) implementation 
            of <a href="http://www.json.org/"><acronym title="JavaScript Object Notation">
                JSON</acronym></a> and <a href="http://www.json-rpc.org/">JSON-RPC</a> for the
            <a href="http://msdn.microsoft.com/netframework/">Microsoft .NET Framework</a>,
            including <a href="http://www.asp.net/">ASP.NET</a>. What can you do with Jayrock?
            In a few words, Jayrock allows clients, typically <a href="http://en.wikipedia.org/wiki/JavaScript">
                JavaScript</a> in web pages, to be able to call into server-side methods
            using JSON as the wire format and JSON-RPC as the procedure invocation protocol.
            The methods can be called synchronously or asynchronously.
        </p>
        <ul>
            <li><a href="http://developer.berlios.de/project/showfiles.php?group_id=4638">Download
                Jayrock 0.9 now!</a></li>
            <li><a href="http://groups.google.com/group/jayrock">Discuss Jayrock</a></li>
            <li><a href="http://jayrock.berlios.de/coverage-report.html">Code coverage report</a></li>
            <li><a href="http://developer.berlios.de/projects/jayrock/">Project foundry on BerliOS</a></li>
            <li><a href="http://www.ohloh.net/projects/3887">Ohloh Metrics Report</a></li>
        </ul>
        <p>
            Compatibility &amp; compliance:</p>
        <p id="compat-logos">            
                <a target="_blank" href="http://www.microsoft.com/windows/"><img src="images/cclogos/windows.gif" width="38" height="35" alt="Microsoft Windows" border="0" /></a>
                <a target="_blank" href="http://www.linux.org/"><img src="images/cclogos/linux.gif" width="34" height="40" alt="Linux" border="0" /></a>
                <a target="_blank" href="http://msdn.microsoft.com/netframework/"><img src="images/cclogos/dotnet.gif" width="68" height="35" alt="Microsoft .NET Framework" border="0" /></a>
                <a target="_blank" href="http://www.go-mono.com/"><img src="images/cclogos/mono.gif" width="40" height="46" alt="Mono" border="0" /></a>
                <a target="_blank" href="http://www.python.org/"><img src="images/cclogos/python.gif" width="39" height="39" alt="Python" border="0" /></a>
                <a target="_blank" href="http://www.microsoft.com/ie/"><img src="images/cclogos/ie.gif" width="46" height="40" alt="Microsoft Internet Explorer" border="0" /></a>
                <a target="_blank" href="http://www.getfirefox.com/"><img src="images/cclogos/ff.gif" width="50" height="44" alt="FireFox" border="0" /></a>
                <a target="_blank" href="http://www.opera.com/"><img src="images/cclogos/opera.gif" width="53" height="43" alt="Opera" border="0" /></a>
                <a target="_blank" href="http://www.opensource.org/docs/definition.php"><img src="images/cclogos/osi-certified.gif" width="60" height="42" alt="Open Source (OSI) Certified" border="0" /></a>
        </p>
        <p>No time for Jayrock right now? Got <a href="http://del.icio.us/">del.icio.us</a>? Bookmark it and come back later&hellip;</p>
        <script type="text/javascript">
            if (typeof window.Delicious == "undefined") window.Delicious = {};
            Delicious.BLOGBADGE_DEFAULT_CLASS = 'delicious-blogbadge-line';
            Delicious.BLOGBADGE_MANUAL_MODE = true;
        </script>
        <script type="text/javascript" src="http://images.del.icio.us/static/js/blogbadge.js"></script>
        <script type="text/javascript">
            Delicious.BlogBadge.writeBadge("delicious-blogbadge-"+Math.random(), "http://jayrock.berlios.de/", document.title, {});
            Delicious.BlogBadge.onload();                
        </script>
        <h1>
            <a id="get-source" name="get-source">Where is the Source, Luke?</a></h1>
        <p>
            You can obtain the latest source of code of Jayrock from the <a href="http://subversion.tigris.org/">
                Subversion</a> repository hosted at <a href="http://www.berlios.de">BerliOS</a>.
            Needless to say, you will need a <a href="http://subversion.tigris.org/project_packages.html">
                Subversion client</a> for your platform (Windows users may also want to check
            out <a href="http://tortoisesvn.tigris.org/">TortoiseSVN</a>, which is a Windows
            Shell Extension for Subversion) to access the repository. If you don't have a Subversion
            client handy and just wish to browse the source code, you can do so online using
            either <a href="http://svn.berlios.de/wsvn/jayrock">WebSVN</a> or <a href="http://svn.berlios.de/viewcvs/jayrock">
                ViewCVS</a>.</p>
        <p>
            For anonymous access to the respository trunk, use <code>svn://svn.berlios.de/jayrock/trunk</code>
            (or <code>http://svn.berlios.de/svnroot/repos/jayrock/trunk</code> for access via HTTP).
            The command-line for the Subversion client would therefore be:</p>
        <p>
            <code>svn checkout svn://svn.berlios.de/jayrock/trunk jayrock</code></p>
        <p>
            The third argument, <code>jayrock</code>, is the directory name where the local
            working copy will be downloaded so this can be another name if you like.</p>
        <p>
            If you want a snapshot of the latest files without bothering to go through the source
            repository then you can simply download them from the <a href="http://developer.berlios.de/project/showfiles.php?group_id=4638">
                Files</a> section of the project.
        </p>
        <h1>
            <a id="compiling" name="compiling">Compiling</a></h1>
        <p>
            Note: For quicker setup instructions, see the section <a href="#setup">Setting Up Jayrock</a>.</p>
        <p>
            Once you have checked out a working copy of <a href="#get-source">the source</a>
            from the respository, you can compile Jayrock in one of two ways. You can either
            open the included <a href="http://msdn.microsoft.com/vstudio/">Microsoft Visual Studio</a>
            solution files and use the IDE to compile the projects or you can use the included
            <a href="http://nant.sourceforge.net/">NAnt</a> build script to compile from the
            command-line.</p>
        <h2>
            Compiling with NAnt</h2>
        <p>
            You do not need NAnt installed on your machine to compile Jayrock. The right and
            required version of all required tools (<a href="http://nant.sourceforge.net/">NAnt</a>,
            <a href="http://www.nunit.org/">NUnit</a>, <a href="http://ncover.org/">NCover</a> and 
            <a href="http://www.kiwidude.com/blog/2006/01/ncoverexplorer-debut.html">NCoverExplorer</a>)
            is already included under the <code>tools</code> directory under Jayrock. If you
            are on the Windows platform, you can simply run the batch file <code>src\build.bat</code>
            to invoke NAnt and have it build all the targets. To invoke NAnt explicitly, otherwise,
            use the following command (assuming you are in the root of the working directory):</p>
        <p>
            <code>tools\nant-0.85\NAnt /f:src\nant.build</code></p>
        <p>
            A full build runs the unit tests, creates a code coverage report from the tests
            and then goes on to compile the debug and release assemblies for Jayrock and Jayrock.Json.</p>
        <p>
            The NAnt script can be used to build 4 different types of so-called <em><a href="http://nant.sourceforge.net/release/latest/help/fundamentals/targets.html">
                targets</a></em>:</p>
        <dl>
            <dt>test </dt>
            <dd>
                Builds the test project and runs the unit tests using NUnit.
            </dd>
            <dt>debug</dt>
            <dd>Builds the debug version of the Jayrock and Jayrock.Json assemblies.</dd><dt>release</dt><dd>Builds
                the release version of the Jayrock and Jayrock.Json assemblies. Debug symbols are
                still included.</dd>
            <dt>cover</dt>
            <dd>
                Builds the test project, and creates report using NCover and NCoverExplorer for code
                coverage resulting from the unit tests. The generated report can be found in <code>src\coverage-report.htm</code>.
            </dd>
        </dl>
        <h2>
            Compiling with Visual Studio</h2>
        <p>
            Jayrock comes with <a href="http://msdn.microsoft.com/vstudio/previous/2003/">Microsoft
                Visual Studio 2003</a> project and solution files that compile assemblies for
            Microsoft .NET Framework 1.1. There is little to know except open the desired solution
            in Visual Studio and build away! The three solutions that you will find under <code>
                src</code> are:</p>
        <dl>
            <dt>Jayrock.Json</dt>
            <dd>Solution that builds and contains functionality related to JSON only, 
                without all the JSON-RPC bells and whistles. If you are looking
                to only work with the JSON data format then this is the right solution for you.</dd>
            <dt>Jayrock</dt>
            <dd>The complete solution that includes and build the JSON-RPC and JSON bits.</dd>
            <dt>Jayrock.Tests</dt>
            <dd>
                Solution that contains a test-view of the project, containing references and sources
                for unit tests.</dd>
        </dl>
        <h2>
            Notes for Visual Studio 2002 &amp; 2005</h2>
        <p>
            If you are using Microsoft Visual Studio .NET 2002, you can use the <a href="http://www.codeproject.com/macro/vsconvert.asp">
                Visual Studio Converter</a> utility to convert the 2003 solution files back
            to the older format and compile for Microsoft .NET Framework 1.0.</p>
        <p>
            If you are working with Microsoft .NET Framework 2.0, you do not need re-compile Jayrock
            for it. The 1.1 assemblies will run just fine as they are. If you wish to open the
            solution with Microsoft Visual Studio 2005 nonetheless, you can open the 2003 solution
            files in 2005 have them upgraded automatically by the <a href="http://msdn2.microsoft.com/en-us/library/60z6y467.aspx">
                Visual Studio Conversion Wizard</a>. You will see a few warnings after the upgrade
            due to some APIs that have been obsoleted in Microsoft .NET Framework 2.0. These
            are <a href="http://en.wikipedia.org/wiki/Mostly_Harmless">mostly harmless</a>.</p>
        <h1>
            <a id="contributing" name="contributing">Contributing to the Project</a></h1>
        <div id="osi-logo">
            <a href="http://www.opensource.org/docs/definition.php"><img border="0"  
               src="http://opensource.org/trademarks/osi-certified/web/osi-certified-60x50.gif" 
               alt="Open Source (OSI) Certified"
               width="60" height="50" /></a>
        </div>            
        <p>
            Jayrock is provided as open source and free software (as per 
            <a href="http://www.opensource.org/docs/definition.php">Open Source Definition</a> and under 
            <a href="http://www.opensource.org/licenses/lgpl-license.php"><acronym title="GNU Lesser General Public License">LGPL</acronym></a>) for two principal reasons. First, an open source
            community provides a way for individuals and companies to collaborate on projects
            that none could achieve on their own. Second, the open source model has the technical
            advantage of turning users into potential co-developers. With source code readily
            available, users can help debug quickly and promote rapid code enhancements. In
            short, <strong>you are encouraged and invited to contribute!</strong>
        </p>
        <p>
            Please <a href="http://www.raboof.com/contact.aspx">contact Atif Aziz</a> (principal
            developer and project leader) if you are interested in contributing.</p>
        <p>
            You don't have to necessarily contribute in the form of code submissions only. Contributions
            are appreciated and needed in any of the following forms:</p>
        <ul>
            <li>Help diagnose and <a href="http://developer.berlios.de/bugs/?group_id=4638">report
                problems</a></li>
            <li>Suggest fixes or <a href="http://developer.berlios.de/patch/?group_id=4638">send
                in your patches</a></li>
            <li>Improve the code</li>
            <li>Help with unit and end-to-end testing</li>
            <li>Provide <a href="#community">peer support on mailing lists, forums or newsgroups</a>.
            </li>
        </ul>
        <p>
            For more information, see <a href="http://developer.berlios.de/projects/jayrock/">Jayrock
                on BerliOS</a>.</p>
        <h2>
            How Else?</h2>
        <p>
            The above things require time and energy and if you can donate it then there is
            nothing better for Jayrock. Honestly! On the other hand, there is only this to consider.
            Has Jayrock helped you in a project at your daytime job? Well, a lot of companies
            happily profit from open source projects in terms of time and money (especially
            if time is money), so talk to your manager or development lead about making a <em>small</em>
            donation. If you or your company wish to be listed as a donor, then send along any
            or all of the following information to be directly listed here: your name, company
            you work for and a link to your and/or company's home page (would also be nice to
            know the country). Thanks!</p>
        <form action="https://www.paypal.com/cgi-bin/webscr" method="post">
            <input type="hidden" name="cmd" value="_s-xclick" />
            <input type="image" src="https://www.paypal.com/en_US/i/btn/x-click-but04.gif" name="submit" alt="Make donation with PayPal to Jayrock" />
            <img alt="" border="0" src="https://www.paypal.com/en_US/i/scr/pixel.gif" width="1" height="1" />
            <input type="hidden" name="encrypted" value="-----BEGIN PKCS7-----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-----END PKCS7-----" />
        </form>
        <h1>
            <a id="community" name="community">Community &amp; Discussions</a></h1>
        <p>
            Jayrock is an open source project and so naturally relies on to build, leverage
            and enjoy peer support from a community of users with a shared interest in the project.
            Assessing if Jayrock is right the right solution for you? Having trouble using some
            aspect of it? Want to simply provide feedback or ideas for further development?
            Then <a href="http://groups.google.com/group/jayrock">Jayrock Google Group</a> is
            the place to go.
        </p>
        <table style="border: 1px solid #aa0033; font-size: small" align="center">
            <tr>
                <td rowspan="2">
                    <img src="http://groups.google.com/groups/img/groups_medium.gif" height="58" width="150"
                        alt="Google Groups" />
                </td>
                <td align="center">
                    <b>Jayrock</b></td>
            </tr>
            <tr>
                <td align="center">
                    <a href="http://groups.google.com/group/jayrock">Browse Archives</a> at <a href="http://groups.google.com">
                        groups.google.com</a>
                </td>
            </tr>
        </table>
        <h1>
            <a id="setup" name="setup">Setting Up Jayrock</a></h1>
        <ol>
            <li>Setup a virtual directory and application in IIS called <code>jayrock</code> that
                points to the directory <code>www</code> under your working copy of Jayrock.</li>
            <li>Open the Visual Studio .NET 2003 solution called <code>Jayrock Web</code> to compile all 
                projects. There is also a <a href="http://nant.sourceforge.net/">NAnt</a> 
                0.85 build script but this builds all other projects except the web project. If you 
                compile from the command-line, the only additional step required at the moment is to 
                manually copy the <code>Jayrock.dll</code> and <code>Jayrock.Json.dll</code> from the
                <code>bin</code> directory in the root of your working copy to <code>bin</code>
                directory under <code>www</code>.</li>
            <li>Open up a browser window and navigate to
                the virtual root created in the first step, which most probably reads something like 
                <code><span class="fake-a">http://localhost/jayrock/</span></code>).</li>
        </ol>
        <h1>
            <a id="quick-start" name="quick-start">ASP.NET Quick Start</a></h1>
        <p class="note">
            <strong>IMPORTANT!</strong> This quick start tutorial and its code illustrations are
            based on version <% = typeof(Jayrock.Json.JsonReader).Assembly.GetName().Version %> of Jayrock.
            If you are using an older build then some of this tutorial may not make sense 
            or work. In that case, use the tutorial supplied with the version you have 
            instead if you cannot upgrade right away.</p>
        <p>
            To use Jayrock in your ASP.NET project, add a reference to the 
            <code>Jayrock.dll</code> and <code>Jayrock.Json.dll</code> assemblies add a copy of 
            <code>json.js</code> (distributed with Jayrock and found in <code>www</code>
            subdirectory) to the root of your web. A JSON-RPC service is best exposed using Jayrock
            by creating an <a href="http://msdn.microsoft.com/library/en-us/cpguide/html/cpconhttpruntimesupport.asp">
                ASP.NET HTTP handler</a>. In this quick start, we will create a JSON-RPC service
            called <code>HelloWorld</code>. Begin by creating a file called <code>helloworld.ashx</code>
            in the root your ASP.NET application. Add the following code to the file:
        </p>
        <pre class="code">&lt;%@ WebHandler Class="JayrockWeb.HelloWorld" %&gt;

namespace JayrockWeb
{
    using System;
    using System.Web;
    using Jayrock.Json;
    using Jayrock.JsonRpc;
    using Jayrock.JsonRpc.Web;

    public class HelloWorld : JsonRpcHandler
    {
        [ JsonRpcMethod("greetings") ]
        public string Greetings()
        {
            return "Welcome to Jayrock!";
        }
    }
}
</pre>
        <p>
            There are a few interesting things to note about this code. First of all, <code>HelloWorld</code>
            inherits from the <code>Jayrock.JsonRpc.Web.JsonRpcHandler</code> class. This is
            all that is needed to make your service callable using the standard JSON-RPC protocol.
            Second, the <code>Greetings</code> method is decorated with the <code>JsonRpcMethod</code>
            attribute. This is required to tell Jayrock that your method should be callable
            over JSON-RPC. By default, public methods of your class are not exposed automatically.
            Next, the first parameter to the <code>JsonRpcMethod</code> attribute is the client-side
            name of the method, which in this case happens to be <code>greetings</code>. This
            is optional, and if omitted, will default to the actual method name as it appears
            in the source (that is, <code>Greetings</code> with the capital letter G). Since
            <a href="http://en.wikipedia.org/wiki/Javascript">JavaScript</a> programs usually
            adopt the <a href="http://en.wikipedia.org/wiki/Camel_Case">camel case</a> naming
            convention, providing an alternate and client-side version of your method's internal
            name via the <code>JsonRpcMethod</code> attribute is always a good idea. You are
            now almost ready to test your service. The last item needed is the addition of a
            few sections in the <code>web.config</code> of your ASP.NET application:
        </p>
        <p class="note">
            <strong>Note:</strong> As of version 0.9.8316, the configuration shown
            here is the assumed default and not required in <code>web.config</code> anymore.
            For the purpose of this tutorial, you may skip adding the following 
            sections to your <code>web.config</code>.</p>
        <pre class="code">&lt;configSections&gt;
    ...
    &lt;sectionGroup name="jayrock"&gt;
        &lt;sectionGroup name="jsonrpc"&gt;
            &lt;section 
                name="features" 
                type="Jayrock.JsonRpc.Web.JsonRpcFeaturesSectionHandler, Jayrock" /&gt;        
        &lt;/sectionGroup&gt;
    &lt;/sectionGroup&gt;
    ...
&lt;/configSections&gt;
...
&lt;jayrock&gt;
    &lt;jsonrpc&gt;
        &lt;features&gt;
            &lt;add name="rpc" 
                 type="Jayrock.JsonRpc.Web.JsonRpcExecutive, Jayrock" /&gt;
            &lt;add name="getrpc" 
                 type="Jayrock.JsonRpc.Web.JsonRpcGetProtocol, Jayrock" /&gt;
            &lt;add name="proxy" 
                 type="Jayrock.JsonRpc.Web.JsonRpcProxyGenerator, Jayrock" /&gt;
            &lt;add name="pyproxy" 
                 type="Jayrock.JsonRpc.Web.JsonRpcPythonProxyGenerator, Jayrock" /&gt;
            &lt;add name="help" 
                 type="Jayrock.JsonRpc.Web.JsonRpcHelp, Jayrock" /&gt;
            &lt;add name="test" 
                 type="Jayrock.JsonRpc.Web.JsonRpcTester, Jayrock" /&gt;
        &lt;/features&gt;
    &lt;/jsonrpc&gt;
&lt;/jayrock&gt;
...
</pre>
        <p>
            The above configuration lines enable various <em>features</em> on top of your service. 
            These features are accessed by using the feature's name in the query string to your handler's
            URL, as in <code>?<em>feature</em></code> (very similar to <a href="http://msdn.microsoft.com/library/en-us/vbcon/html/vbtskExploringWebService.asp">
                how you request the WSDL document for an ASP.NET Web Service</a>). First and
            foremost, there is the <code>rpc</code> feature. It is responsible for actually
            making the JSON-RPC invocation on your service upon receiving a request over HTTP POST. 
            Without this feature, your service is JSON-RPC ready but won't be callable by anyone. 
            The <code>getrpc</code> feature is similar except it makes the services methods
            callable over HTTP GET.</p>
        <p>The <code>proxy</code> feature
            dynamically generates JavaScript code for the client-side proxy. This code will
            contain a class that you can instantiate and use to call the server methods either
            synchronously and asynchronously. In an HTML page, you can import the proxy by using
            your handler's URL as the script source and using <code>?proxy</code> as the query
            string:</p>
        <div>
            <pre class="code">&lt;script 
    type="text/javascript" 
    src="http://localhost/foobar/helloworld.ashx?proxy"&gt;
&lt;/script&gt;</pre>
        </div>
        <p>
            The <code>help</code> feature provides a simple help page in HTML that provides
            a summary of your service and methods it exposes. Finally, the <code>test</code>
            feature provides a simple way to test the methods of your service from right within
            the browser. This is exactly what we are going to work with next. Open up a browser
            window and point it to the URL of your ASP.NET handler. For example, if your ASP.NET
            application is called <code>foobar</code> and is running on your local machine,
            then type <code><span class="fake-a">http://localhost/foobar/helloworld.ashx</span></code>.
            You should now see a page appear that lists the methods exposed by your service:</p>
        <img border="0"  class="figure" src="images/HelloWorldHelp.jpg" width="800" height="600" alt="HelloWorld Help" />
        <p>
            Notice that there are two methods, namely <code>greetings</code> and <code>system.listMethods</code>.
            The <code>system.listMethods</code> is always there and inheirted by all services
            that inherit from the <code>JsonRpcHandler</code> class. It provides <a href="http://scripts.incutio.com/xmlrpc/introspection.html">
                introspection similar to some XML-RPC implementations</a>. As this point, you
            are looking at the help page generated by the help feature. Notice, though, you
            did not have to specify the <code>?help</code> query string. That's because <code>JsonRpcHandler</code>
            defaults to the help feature when it sees a plain <a href="http://www.w3.org/Protocols/rfc2616/rfc2616-sec9.html#sec9.3">HTTP
                GET</a> request for your service. You should also be able to see a link to the test
            page from where you can invoke and test each individual method. Click on this link
            now, which should yield a page similar to the one shown here:
        </p>
        <img border="0"  class="figure" src="images/HelloWorldTest.jpg" width="800" height="600" alt="HelloWorld Test" />
        <p>
            To see if everything is working correctly, select the <code>greetings</code> method
            from the drop-down list and click the button labeled <code>Test</code>. If all goes
            well, you should see the string <code>"Welcome to Jayrock!"</code> returned in the
            response box of the page.</p>
        <p>
            Great. So far we have the service running and tested. Now it is time to try and
            call the service from JavaScript within a web page. In the same directory as where
            you placed <code>helloworld.ashx</code>, create a plain text file called <code>hello.html</code>
            and put the following HTML source in there:</p>
        <pre class="code">&lt;!DOCTYPE html PUBLIC &quot;-//W3C//DTD XHTML 1.0 Transitional//EN&quot; 
    &quot;http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd&quot;&gt;
&lt;html xmlns=&quot;http://www.w3.org/1999/xhtml&quot; lang=&quot;en&quot; xml:lang=&quot;en&quot;&gt;
&lt;head&gt;
    &lt;title&gt;Hello Jayrock&lt;/title&gt;
    &lt;script type="text/javascript" src="json.js"&gt;&lt;/script&gt;
    &lt;script type="text/javascript" src="helloworld.ashx?proxy"&gt;&lt;/script&gt;
    &lt;script type="text/javascript"&gt;
/* &lt;![CDATA[ */

window.onload = function() 
{
    var s = new HelloWorld();

    alert("sync:" + s.greetings());

    s.greetings(function(response) { 
      alert("async:" + response.result) 
    });
}

/* ]]&gt; */
    &lt;/script&gt;
&lt;/head&gt;
&lt;body&gt;
    &lt;p&gt;This page tests the HelloWorld service with Jayrock.&lt;/p&gt;
&lt;/body&gt;
</pre>
        <p>
            This page references two scripts. The first one is <code>json.js</code>, (distributed
            with Jayrock) which contains code for converting JavaScript values into JSON text
            and vice versa. This is required by the second script reference, which points to
            the JavaScript proxy that will be dynamically generated by Jayrock from the server
            so that we can call the service. The third script is embedded in the page and causes
            the <code>greetings</code> method to be called on the HelloWorld service twice.
            In its first form, <code>greetings</code> is called synchronously whereas in its
            second form, it is called asynchronously. How is that? If the proxy sees that the
            last parameter supplied is a JavaScript function then it treats it as a <em>callback</em>.
            The call is made to the service and control returned immediately. When the response
            becomes available, the proxy invokes the callback function and sends it the JSON-RPC
            response object as the first and only parameter. The <code>result</code> property
            of this object can then be consulted to obtain the return value from the server-side
            RPC method. On failure, the response object instead contains an <code>error</code>
            property.</p>
        <p>
            Back in the browser, type the URL <code><span class="fake-a">http://localhost/jayrock/hello.html</span></code>
            in the address bar. As soon as the page loads, you should see two message boxes
            show up one after the other and which display the string <code>"Welcome to Jayrock!"</code>
            returned from our JSON-RPC service method <code>greetings</code>. The first message
            box will be from the synchronous execution whereas the second from the asynchronous
            one.</p>
        <p>
            That's it! We're done. Hope you enjoyed the quick tour.</p>
        <!--
        <h1>
            Inside Jayrock</h1>
        <h2>
            Writing JSON</h2>
        <p>
            To be completed...</p>
        <h2>
            Reading JSON</h2>
        <p>
            To be completed...</p>
        <h2>
            Formatting Types into JSON</h2>
        <p>
            To be completed...</p>
        <h2>
            Importing Types from JSON</h2>
        <p>
            To be completed...</p>
        -->            
        <h1>
            <a id="samples" name="samples">Samples &amp; Demos</a></h1>
        <p>
            You can find a number of JSON-RPC methods demonstrating various features in the
            supplied demo service. See <code><span class="fake-a">http://localhost/jayrock/demo.ashx</span></code>
            on your machine for a working copy of the demo.
        </p>
        <p>
            Note that some of the methods on the demo service, which illustrate data access,
            assume that you have a default instance of <a href="http://www.microsoft.com/sql/">Microsoft
                SQL Server</a> running on your machine with the Northwind database loaded.
        </p>
        <h1>
            <a id="docs" name="docs">Documents</a></h1>
        <dl>
            <dt><a href="Jayrock.pdf">Jayrock Project Presentation</a></dt>
            <dd>
                This presentation contains illustrations that briefly cover the architecture of
                Jayrock's JSON and JSON-RPC implementations. Beware, however, that some bits
                may be obsolete now since the presentation is based on a very early build.
            </dd>
        </dl>
        <h1>
            <a id="qa" name="qa">Got Questions?</a></h1>
        <p>
            If you don't see your questions or concerns addressed below, then try over at the
            <a href="http://groups.google.com/group/jayrock">Jayrock discussion group</a>.</p>
        <dl>
            <dt>What is Jayrock?</dt><dd>Jayrock is a modest and an open source
                implementation of <acronym title="JavaScript Object Notation">
                    JSON</acronym> and JSON-RPC for the Microsoft .NET Framework, including ASP.NET.
                    What's so <em>modest</em> about it? Well, modest as in plain and basic and
                    no work of genius.
            </dd>
            <dt>What can I do with Jayrock?</dt>
            <dd>
                Two things come to mind:
                <ol>
                    <li>You can use just the Jayrock's JSON infrastructure for manipulating JSON data and text without
                    all the JSON-RPC fuss. Just use the stand-alone <code>Jayrock.Json</code> assembly.</li>
                        <li>In addition to the above, you can use Jayrock to expose light-weight services with procedures
                            from within your ASP.NET application. You can then invoke the procedures on those
                            services over HTTP using JSON-RPC as the protocol. A typical use case would be some
                            JavaScript code embedded inside a
                            web page calling back into your services on the web server.</li></ol>
            </dd>
            <dt>So wait, is Jayrock yet another Ajax framework?</dt>
            <dd>
                That depends on what fits your bill for or definition and expectation of an Ajax
                framework. While you can certainly use Jayrock to write rich and interactive web
                page enabled by the <a href="http://en.wikipedia.org/wiki/AJAX">Ajax</a> style of
                development, you'll be a more enlightened soul to believe that it has a wider applicability.
                You can build light-weight services in ASP.NET and then deploy them on your web server, but beyond that,
                any client with HTTP capability, be that scripts or console applications, can benefit
                from Jayrock by remotely invoking procedures of your services. If you have a
                JSON-RPC client library for your language, environment or platform then all the
                better. If not, Jayrock comes with one for JavaScript to get you started off the ground in 
                <a href="http://www.microsoft.com/ie">Microsoft Internet Explorer</a>,
                <a href="http://www.firefox.com/">FireFox</a> and even <a href="http://en.wikipedia.org/wiki/Windows_Script_Host">WSH (Windows Script Host)</a>.
                Given a service, Jayrock can dynamically provide a JavaScript proxy that
                implements the JSON-RPC protocol
                to call back into your service (synchronously and asynchronously). Jayrock, however, does not provide any client-side whiz-bang
                widgets or controls that you may have come to generally expect from other and more ambitious Ajax frameworks.
                For that, you are recommended to shop around elsewhere, like <a href="http://dojotoolkit.org/">
                    Dojo toolkit</a>, <a href="http://ajax.asp.net/">Microsoft ASP.NET AJAX</a>, <a href="http://developer.yahoo.com/yui/">
                        Yahoo! UI Library</a> or <a href="http://ajaxpatterns.org/Ajax_Frameworks">many others</a>.</dd>
            <dt>Which versions of the Microsoft .NET Framework are supported?</dt><dd>Jayrock
      is compiled and delivered for Microsoft .NET Framework 1.1, but it can
      be run against any version of Microsoft .NET Framework, including 1.0 and 2.0. For
      version 1.0, you will have to recompile the binaries. For version 2.0, on the other
      hand, you don't need to do anything. Just toss the assemblies at your application
      and you are good to go.</dd><dt>What is JSON?</dt><dd><a href="http://www.json.org/"><acronym title="JavaScript Object Notation">JSON</acronym></a> stands for JavaScript Object Notation. It is a simple, human-readable,
                text-based and portable data format that is ideal for representing and exchanging
                application data. It has only 5 data types (Boolean, Number, String, Object
                and Array) that are commonly used across a wide number of applications and programming
                languages. For more information, see <a href="http://www.ietf.org/rfc/rfc4627.txt">
                    RFC 4627</a>.
            </dd>
            <dt>What is JSON-RPC?</dt><dd><a href="http://www.json-rpc.org/">JSON-RPC</a> is a light-weight
                remote procedure call protocol that relies on
                JSON for the wire format. All it does
                is provide a simple way to express a call frame as a JSON Object and the result
                or an error resulting from the invocation as another JSON Object. For details, see
                the <a href="http://json-rpc.org/wiki/specification">JSON-RPC specification</a>.&nbsp;</dd></dl>
        <hr />
        <p>
            Updated on
            <% = DateTime.Now.ToLongDateString() %>. Style and design by <a href="http://www.raboof.com/">Atif Aziz</a>.</p>
        <p>
            <a href="http://developer.berlios.de"><img border="0"  
               src="http://developer.berlios.de/bslogo.php?group_id=0" width="124" height="32"
               alt="BerliOS Logo" /></a> 
            <a href="http://validator.w3.org/check?uri=referer"><img border="0"  
               src="http://www.w3.org/Icons/valid-xhtml10" alt="Valid XHTML 1.0 (Transitional)"
               height="31" width="88" /></a>
        </p>
    </div>
</body>
</html>
