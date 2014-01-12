<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="register.aspx.cs" Inherits="Cobalt.register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Sign Up</title>
    <link href="theme/vista/css/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="theme/vista/css/layout.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" DisableCreatedUser="True"
                DisplaySideBar="True" LoginCreatedUser="False" EnableTheming="False" RenderOuterTable="False"
                OnCreatedUser="createdUser">
                <CreateUserButtonStyle CssClass="btn" />
                <WizardSteps>
                    <asp:CreateUserWizardStep runat="server" EnableTheming="False">
                        <ContentTemplate>
                            Sign Up for Your New Account
                            <div class="control-group">
                                <asp:Label ID="UserNameLabel" CssClass="control-label" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="UserName" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName"
                                        ErrorMessage="User Name is required." ToolTip="User Name is required." ForeColor="#FF0066">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="PasswordLabel" CssClass="control-label" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password"
                                        ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="CreateUserWizard1"
                                        ForeColor="#FF0066">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="ConfirmPasswordLabel" CssClass="control-label" runat="server" AssociatedControlID="ConfirmPassword">Confirm Password:</asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" ControlToValidate="ConfirmPassword"
                                        ErrorMessage="Confirm Password is required." ToolTip="Confirm Password is required."
                                        ValidationGroup="CreateUserWizard1" ForeColor="#FF0066">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="EmailLabel" CssClass="control-label" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="EmailRequired" runat="server" ControlToValidate="Email"
                                        ErrorMessage="E-mail is required." ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1"
                                        ForeColor="#FF0066">*</asp:RequiredFieldValidator>
                                </div>
                                <div class="control-group">
                                    <asp:Label ID="ConfirmEmailLabel" CssClass="control-label" AssociatedControlID="ConfirmEmail"
                                        runat="server" Text="Confirm Email"></asp:Label>
                                    <div class="controls">
                                        <asp:TextBox ID="ConfirmEmail" runat="server"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="ConfirmEmailRequired" runat="server" ControlToValidate="ConfirmEmail"
                                            ErrorMessage="Confirm E-mail is required." ToolTip="Confirm E-mail is required."
                                            ValidationGroup="CreateUserWizard1" ForeColor="#FF0066">*</asp:RequiredFieldValidator>
                                    </div>
                                </div>
                                <div class="control-group">
                                    <asp:Label ID="QuestionLabel" CssClass="control-label" runat="server" AssociatedControlID="Question">Security Question:</asp:Label>
                                </div>
                                <div class="controls">
                                    <asp:TextBox ID="Question" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="QuestionRequired" runat="server" ControlToValidate="Question"
                                        ErrorMessage="Security question is required." ToolTip="Security question is required."
                                        ValidationGroup="CreateUserWizard1" ForeColor="#FF0066">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="controls">
                                <asp:Label ID="AnswerLabel" CssClass="control-label" runat="server" AssociatedControlID="Answer">Security Answer:</asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="Answer" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="AnswerRequired" runat="server" ControlToValidate="Answer"
                                        ErrorMessage="Security answer is required." ToolTip="Security answer is required."
                                        ValidationGroup="CreateUserWizard1" ForeColor="#FF0066">*</asp:RequiredFieldValidator>
                                </div>
                            </div>
                            <div class="control">
                                <asp:ValidationSummary ID="ValidationSummary1" CssClass="alert alert-error" runat="server"
                                    DisplayMode="List" ValidationGroup="CreateUserWizard1" />
                                <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                            </div>
                            <div class="controls">
                                <asp:RegularExpressionValidator ID="PasswordLength" runat="server" ControlToValidate="Password"
                                    CssClass="alert alert-error" ErrorMessage="Use Strong Password Ex: Hello12#"
                                    ToolTip="Use Strong Password" ValidationExpression="^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$">Use Strong Password</asp:RegularExpressionValidator>
                            </div>
                            <div class="controls">
                                <asp:RegularExpressionValidator ID="InvaliedEmail" runat="server" ControlToValidate="Email"
                                    CssClass="alert alert-error" ErrorMessage="Invalied Email" ToolTip="Invalied Email"
                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">Invalied Email</asp:RegularExpressionValidator>
                            </div>
                            <div class="controls">
                                <asp:CompareValidator ID="EmailCompare" CssClass="alert alert-error" runat="server"
                                    ControlToCompare="Email" ControlToValidate="ConfirmEmail" ErrorMessage="CompareValidator">The Email and Confirmation Email must match.</asp:CompareValidator>
                            </div>
                            <div class="controls">
                                <asp:CompareValidator ID="PasswordCompare" CssClass="alert alert-error" runat="server"
                                    ControlToCompare="Password" ControlToValidate="ConfirmPassword" Display="Dynamic"
                                    ErrorMessage="The Password and Confirmation Password must match." ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
                            </div>
                        </ContentTemplate>
                    </asp:CreateUserWizardStep>
                    <asp:CompleteWizardStep runat="server" EnableTheming="False">
                        <ContentTemplate>
                            Thank you for registering. Please check your email for a confirmation request with
                            a link that will confirm your account. Once you click the link, your registration
                            will be complete.
                            <asp:Button ID="ContinueButton" CssClass="btn" runat="server" CausesValidation="False"
                                CommandName="Continue" Text="Continue" ValidationGroup="CreateUserWizard1" />
                            </td>
                        </ContentTemplate>
                    </asp:CompleteWizardStep>
                </WizardSteps>
                <FinishPreviousButtonStyle CssClass="btn" />
            </asp:CreateUserWizard>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
    <script src="theme/vista/js/jquery.js" type="text/javascript"></script>
    <script src="theme/vista/js/bootstrap.min.js" type="text/javascript"></script>
</body>
</html>