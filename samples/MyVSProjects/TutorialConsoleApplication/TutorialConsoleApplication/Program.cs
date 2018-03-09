﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataBox;
using Microsoft.Azure.Management.DataBox.Models;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Azure.Authentication;

namespace TutorialConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Azure Databox Operations:");
            Console.WriteLine("  1 - Get job");
            Console.WriteLine("  2 - List jobs");
            Console.WriteLine("  3 - List jobs by resource group");
            Console.WriteLine("  4 - Validate shipping address");
            Console.WriteLine("  5 - Create job");
            Console.WriteLine("  6 - Cancel job");
            Console.WriteLine("  7 - Delete job");
            Console.WriteLine("  8 - Download shipping label uri");
            Console.WriteLine("  9 - Get region availability");
            Console.WriteLine(" 10 - Book shipment pickup");
            Console.WriteLine(" 11 - Get copy logs uri");
            Console.WriteLine(" 12 - Get secrets");
            Console.Write("\nChoose an option (1 to 12): ");

            string action = Console.ReadLine();

            switch (action)
            {
                case "1":
                    GetJob();
                    break;

                case "2":
                    ListJobs();
                    break;

                case "3":
                    ListJobsByResourceGroup();
                    break;

                case "4":
                    ValidateShippingAddress();
                    break; ;

                case "5":
                    CreateJob();
                    break;

                case "6":
                    CancelJob();
                    break;

                case "7":
                    DeleteJob();
                    break;

                case "8":
                    DownloadShippingLableUri();
                    break;

                case "9":
                    GetRegionAvailability();
                    break;

                case "10":
                    BookShipmentPickup();
                    break;

                case "11":
                    GetCopyLogsUri();
                    break;

                case "12":
                    GetSecrets();
                    break;
            }
        }

        private static string subscriptionId;
        private static string tenantId;
        private static string aadApplicationId;
        private static string aadApplicationKey;

        static DataBoxManagementClient InitializeDataBoxClient()
        {
            const string frontDoorUrl = "https://login.microsoftonline.com";
            const string tokenUrl = "https://management.azure.com";

            // Setup the configuration parameters.
            subscriptionId = "3c66da21-607e-49b4-8bdf-f25fbb8f705f"; // "3c66da21-607e-49b4-8bdf-f25fbb8f705f";    //"d3ebfe71 -b7a9-4c57-92b9-68a2afde4de5";            // Input Subscription ID.
            tenantId = "72f988bf-86f1-41af-91ab-2d7cd011db47";               // Input the TenantID of the subscription.
            aadApplicationId = "59eee99c-edf6-40ad-a049-cfd4e7f5062f";      // Input application ID for which the service principal was set.
            aadApplicationKey = "MNtOEHyY37aNV/eg2Fxtlyov3+52abEUP8d3UaHiNvc=";    // Input application authentication key for which the AAD application.

            // Validates AAD ApplicationId and returns token
            var credentials = ApplicationTokenProvider.LoginSilentAsync(
                                tenantId,
                                aadApplicationId,
                                aadApplicationKey,
                                new ActiveDirectoryServiceSettings()
                                {
                                    AuthenticationEndpoint = new Uri(frontDoorUrl),
                                    TokenAudience = new Uri(tokenUrl),
                                    ValidateAuthority = true,
                                }).GetAwaiter().GetResult();

            // Initializes a new instance of the DataBoxManagementClient class.
            DataBoxManagementClient dataBoxManagementClient = new DataBoxManagementClient(credentials);

            // Set SubscriptionId
            dataBoxManagementClient.SubscriptionId = subscriptionId;

            return dataBoxManagementClient;
        }

        private static void GetJob()
        {
            string resourceGroupName = "harirg";
            string jobName = "hari-test-databox-1";
            string expand = "details";

            //Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            // Gets information about the specified job.
            JobResource jobResource = JobsOperationsExtensions.Get(dataBoxManagementClient.Jobs, resourceGroupName, jobName, expand);
        }

        private static void ListJobs()
        {
            //Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            IPage<JobResource> jobPageList = null;
            List<JobResource> jobList = new List<JobResource>();

            do
            {
                // Lists all the jobs available under the subscription.
                if (jobPageList == null)
                {
                    jobPageList = JobsOperationsExtensions.List(dataBoxManagementClient.Jobs);
                }
                else
                {
                    jobPageList = JobsOperationsExtensions.ListNext(dataBoxManagementClient.Jobs, jobPageList.NextPageLink);
                }

                jobList.AddRange(jobPageList.ToList());

            } while (!(string.IsNullOrEmpty(jobPageList.NextPageLink)));
        }

        private static void ListJobsByResourceGroup()
        {
            //Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            IPage<JobResource> jobPageList = null;
            List<JobResource> jobList = new List<JobResource>();
            string resourceGroupName = "<resource-group-name>";

            do
            {
                // Lists all the jobs available under resource group.
                if (jobPageList == null)
                {
                    jobPageList = JobsOperationsExtensions.ListByResourceGroup(dataBoxManagementClient.Jobs, resourceGroupName);
                }
                else
                {
                    jobPageList = JobsOperationsExtensions.ListByResourceGroupNext(dataBoxManagementClient.Jobs, jobPageList.NextPageLink);
                }

                jobList.AddRange(jobPageList.ToList());

            } while (!(string.IsNullOrEmpty(jobPageList.NextPageLink)));
        }

        private static void ValidateShippingAddress()
        {
            AddressType addressType = AddressType.None;
            string companyName = "<company-name>";
            string streetAddress1 = "<stree-address-1>";
            string streetAddress2 = "<stree-address-2>";
            string streetAddress3 = "<stree-address-3>";
            string postalCode = "<postal-code>";
            string city = "<city>";
            string stateOrProvince = "<state-or-province>";
            CountryCode countryCode = CountryCode.US;

            ShippingAddress shippingAddress = new ShippingAddress()
            {
                AddressType = addressType,
                CompanyName = companyName,
                StreetAddress1 = streetAddress1,
                StreetAddress2 = streetAddress2,
                StreetAddress3 = streetAddress3,
                City = city,
                StateOrProvince = stateOrProvince,
                PostalCode = postalCode,
                Country = countryCode.ToString(),
            };

            // Set location of the resource
            string location = "<location>";

            // Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();
            dataBoxManagementClient.Location = location;

            ValidateAddress validateAddress = new ValidateAddress(shippingAddress, DeviceType.Pod);
            AddressValidationOutput addressValidationOutput = ServiceOperationsExtensions.ValidateAddressMethod(dataBoxManagementClient.Service, validateAddress);
        }

        private static void CreateJob()
        {
            AddressType addressType = AddressType.None;
            string streetAddress1 = "<stree-address-1>";
            string streetAddress2 = "<stree-address-2>";
            string streetAddress3 = "<stree-address-3>";
            string postalCode = "<postal-code>";
            string city = "<city>";
            string stateOrProvince = "<state-or-province>";
            CountryCode countryCode = CountryCode.US;

            ShippingAddress shippingAddress = new ShippingAddress()
            {
                StreetAddress1 = streetAddress1,
                StreetAddress2 = streetAddress2,
                StreetAddress3 = streetAddress3,
                AddressType = addressType,
                Country = countryCode.ToString(),
                PostalCode = postalCode,
                City = city,
                StateOrProvince = stateOrProvince,
            };

            string emailIds = "<email-id>";
            string phoneNumber = "<phone-number>";
            string contactName = "<contact-name>";

            List<string> emailIdList = new List<string>();
            emailIdList = emailIds.Split(new char[';'], StringSplitOptions.RemoveEmptyEntries).ToList();

            ContactDetails contactDetails = new ContactDetails()
            {
                Phone = phoneNumber,
                EmailList = emailIdList,
                ContactName = contactName
            };

            string storageAccProviderType = "Microsoft.Storage"; // Microsoft.Storage / Microsoft.ClassicStorage
            string storageAccResourceGroupName = "<storage-account-resource-group-name>";
            string storageAccName = "<storage-account-name>";
            AccountType accountType = AccountType.GeneralPurposeStorage;

            List<DestinationAccountDetails> destinationAccountDetails = new List<DestinationAccountDetails>();
            destinationAccountDetails.Add(new DestinationAccountDetails(string.Concat("/subscriptions/", subscriptionId, "/resourceGroups/", storageAccResourceGroupName, "/providers/", storageAccProviderType, "/storageAccounts/", storageAccName.ToLower()), accountType));

            PodJobDetails jobDetails = new PodJobDetails(contactDetails, shippingAddress);

            string resourceGroupName = "<resource-group-name>";
            string location = "<location>";
            string jobName = "<job-or-order-name>";

            JobResource newJobResource = new JobResource(location, destinationAccountDetails, jobDetails);
            newJobResource.DeviceType = DeviceType.Pod;

            // Initializes a new instance of the DataBoxManagementClient class.
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();
            dataBoxManagementClient.Location = location;

            // Validate shipping address
            AddressValidationOutput addressValidateResult = ServiceOperationsExtensions.ValidateAddressMethod(dataBoxManagementClient.Service, new ValidateAddress(shippingAddress, newJobResource.DeviceType));

            if (addressValidateResult.ValidationStatus != AddressValidationStatus.Valid)
            {
                Console.WriteLine("Address validation status: {0}", addressValidateResult.ValidationStatus);

                if (addressValidateResult.ValidationStatus == AddressValidationStatus.Ambiguous)
                {
                    Console.WriteLine("\nSUPPORT ADDRESSES:");
                    foreach (ShippingAddress address in addressValidateResult.AlternateAddresses)
                    {
                        Console.WriteLine("Address type: {0}", address.AddressType);
                        if (!(string.IsNullOrEmpty(address.CompanyName))) Console.WriteLine("Company name: {0}", address.CompanyName);
                        if (!(string.IsNullOrEmpty(address.StreetAddress1))) Console.WriteLine("Street address1: {0}", address.StreetAddress1);
                        if (!(string.IsNullOrEmpty(address.StreetAddress2))) Console.WriteLine("Street address2: {0}", address.StreetAddress2);
                        if (!(string.IsNullOrEmpty(address.StreetAddress3))) Console.WriteLine("Street address3: {0}", address.StreetAddress3);
                        if (!(string.IsNullOrEmpty(address.City))) Console.WriteLine("City: {0}", address.City);
                        if (!(string.IsNullOrEmpty(address.StateOrProvince))) Console.WriteLine("State/Province: {0}", address.StateOrProvince);
                        if (!(string.IsNullOrEmpty(address.Country))) Console.WriteLine("Country: {0}", address.Country);
                        if (!(string.IsNullOrEmpty(address.PostalCode))) Console.WriteLine("Postal code: {0}", address.PostalCode);
                        if (!(string.IsNullOrEmpty(address.ZipExtendedCode))) Console.WriteLine("Zip extended code: {0}", address.ZipExtendedCode);
                        Console.WriteLine();
                    }
                }
                Console.ReadLine();
                return;
            }

            // Creates a new job.
            JobResource jobResource = JobsOperationsExtensions.Create(dataBoxManagementClient.Jobs, resourceGroupName, jobName, newJobResource);
        }

        private static void CancelJob()
        {
            string resourceGroupName = "<resource-group-name>";
            string jobName = "<job-name>";
            string reason = "<reason>";

            // Initializes a new instance of the DataBoxManagementClient class.
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            CancellationReason cancellationReason = new CancellationReason(reason);

            // Initiate cancel job
            JobsOperationsExtensions.Cancel(dataBoxManagementClient.Jobs, resourceGroupName, jobName, cancellationReason);
        }

        private static void DeleteJob()
        {
            string resourceGroupName = "<resource-group-name>";
            string jobName = "<job-name>";

            // Initializes a new instance of the DataBoxManagementClient class.
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            // Initiate cancel job
            JobsOperationsExtensions.Delete(dataBoxManagementClient.Jobs, resourceGroupName, jobName);
        }

        private static void DownloadShippingLableUri()
        {
            string resourceGroupName = "<resource-group-name>";
            string jobName = "<job-name>";

            // Initializes a new instance of the DataBoxManagementClient class.
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            // Initiate cancel job
            ShippingLabelDetails shippingLabelDetails = JobsOperationsExtensions.DownloadShippingLabelUri(dataBoxManagementClient.Jobs, resourceGroupName, jobName);
            Console.WriteLine("Shipping address sas url: \n{0}", shippingLabelDetails.ShippingLabelSasUri);
            Console.ReadLine();
        }

        private static void GetRegionAvailability()
        {
            string location = "<location>";

            // Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();
            dataBoxManagementClient.Location = location;

            CountryCode countryCode = CountryCode.US;
            RegionAvailabilityInput regionAvailabilityInput = new RegionAvailabilityInput(countryCode, DeviceType.Pod);

            // Initiate to get list of support regions (service and stroage accout)
            //RegionAvailabilityResponse regionAvailabilityResponse = ServiceOperationsExtensions.RegionAvailability(dataBoxManagementClient.Service, regionAvailabilityInput);
            ServiceHealthResponseList regionAvailabilityResponse = ServiceOperationsExtensions.GetServiceHealth(dataBoxManagementClient.Service);

            //// Get list of support regions
            //List<SupportedRegions> supportRegions = new List<SupportedRegions>();
            //supportRegions.AddRange(regionAvailabilityResponse.SupportedRegions);
        }

        private static void BookShipmentPickup()
        {
            string resourceGroupName = "<resoruce-group-name>";
            string jobName = "<job-name>";

            DateTime dtStartTime = new DateTime();
            DateTime dtEndTime = new DateTime();
            string shipmentLocation = "";

            ShipmentPickUpRequest shipmentPickUpRequest = new ShipmentPickUpRequest(dtStartTime, dtEndTime, shipmentLocation);

            // Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            // Initiate Book shipment pick up
            ShipmentPickUpResponse shipmentPickUpResponse = JobsOperationsExtensions.BookShipmentPickUp(
                                                                dataBoxManagementClient.Jobs,
                                                                resourceGroupName,
                                                                jobName,
                                                                shipmentPickUpRequest);

            Console.WriteLine("Confirmation number: {0}", shipmentPickUpResponse.ConfirmationNumber);
        }

        private static void GetCopyLogsUri()
        {
            string resourceGroupName = "<resource-group-name";
            string jobName = "<job-name>";

            // Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            GetCopyLogsUriOutput getCopyLogsUriOutput = JobsOperationsExtensions.GetCopyLogsUri(dataBoxManagementClient.Jobs, resourceGroupName, jobName);

            if (getCopyLogsUriOutput.CopyLogDetails != null)
            {
                Console.WriteLine("Copy log details");
                foreach (AccountCopyLogDetails copyLogitem in getCopyLogsUriOutput.CopyLogDetails)
                {
                    Console.WriteLine(string.Concat("Account name: ", copyLogitem.AccountName, Environment.NewLine, "Copy log link: ", copyLogitem.CopyLogLink, Environment.NewLine, Environment.NewLine));
                }
            }
        }

        private static void GetSecrets()
        {
            string resourceGroupName = "<resource-group-name>";
            string jobName = "<job-name>";

            // Initializes a new instance of the DataBoxManagementClient class
            DataBoxManagementClient dataBoxManagementClient = InitializeDataBoxClient();

            UnencryptedSecrets secrets = ListSecretsOperationsExtensions.ListByJobs(dataBoxManagementClient.ListSecrets, resourceGroupName, jobName);
            PodJobSecrets podSecret = (PodJobSecrets)secrets.JobSecrets;

            if (podSecret.PodSecrets != null)
            {
                Console.WriteLine("Pod device credentails");
                foreach (PodSecret accountCredentials in podSecret.PodSecrets)
                {
                    Console.WriteLine(" Device serial number: {0}", accountCredentials.DeviceSerialNumber);
                    Console.WriteLine(" Device password: {0}", accountCredentials.DevicePassword);

                    foreach (AccountCredentialDetails accountCredentialDetails in accountCredentials.AccountCredentialDetails)
                    {
                        Console.WriteLine("  Account name: {0}", accountCredentialDetails.AccountName);
                        foreach (ShareCredentialDetails shareCredentialDetails in accountCredentialDetails.ShareCredentialDetails)
                        {
                            Console.WriteLine("   Share name: {0}", shareCredentialDetails.ShareName);
                            Console.WriteLine("   User name: {0}", shareCredentialDetails.UserName);
                            Console.WriteLine("   Password: {0}{1}", shareCredentialDetails.Password, Environment.NewLine);
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
