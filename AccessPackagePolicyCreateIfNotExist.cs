using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ap_cli
{
    partial class Program
    {
        static async Task AccessPackagePolicyCreateIfNotExist(GraphServiceClient graphServiceClient, string accessPackageId, string accessPackageDisplayName, string approverGroupId)
        {

            var accessPackageAssignmentPolicy = new AccessPackageAssignmentPolicy
            {
                DisplayName = String.Format("{0} Assignment Policy", accessPackageDisplayName),
                Description = "policy for assignment",
                AllowedTargetScope = AllowedTargetScope.AllConfiguredConnectedOrganizationUsers,
                SpecificAllowedTargets = new List<SubjectSet>()
                {
                },
                Expiration = new ExpirationPattern
                {
                    EndDateTime = null,
                    Duration = null,
                    Type = ExpirationPatternType.NoExpiration
                },
                RequestorSettings = new AccessPackageAssignmentRequestorSettings
                {
                    EnableTargetsToSelfAddAccess = false,
                    EnableTargetsToSelfUpdateAccess = false,
                    EnableTargetsToSelfRemoveAccess = false,
                    AllowCustomAssignmentSchedule = true,
                    EnableOnBehalfRequestorsToAddAccess = false,
                    EnableOnBehalfRequestorsToUpdateAccess = false,
                    EnableOnBehalfRequestorsToRemoveAccess = false,
                    OnBehalfRequestors = new List<SubjectSet>()
                    {
                    }
                },
                RequestApprovalSettings = new AccessPackageAssignmentApprovalSettings
                {
                    IsApprovalRequiredForAdd = true,
                    IsApprovalRequiredForUpdate = false,
                    Stages = new List<AccessPackageApprovalStage>()
                    {new AccessPackageApprovalStage
                        {
                            DurationBeforeAutomaticDenial = new Duration(new TimeSpan(5,0,0,0)),
                            DurationBeforeEscalation = new Duration(new TimeSpan(3,0,0,0)),
                            IsApproverJustificationRequired = true,
                            IsEscalationEnabled = true,
                            EscalationApprovers = new List<SubjectSet>()
                            {
                                new GroupMembers()
                                {
                                    GroupId = approverGroupId
                                }
                            },
                            FallbackEscalationApprovers = new List<SubjectSet>(),
                            PrimaryApprovers = new List<SubjectSet>(),
                            FallbackPrimaryApprovers = new List<SubjectSet>() 
                            {
                                new GroupMembers()
                                {
                                    GroupId = approverGroupId
                                }
                            }
                        }
                    }
                },
                ReviewSettings = new AccessPackageAssignmentReviewSettings
                {
                    IsEnabled = true, // Does not enable the policy
                    ExpirationBehavior = AccessReviewExpirationBehavior.KeepAccess,
                    IsRecommendationEnabled = true,
                    IsReviewerJustificationRequired = true,
                    IsSelfReview = false,
                    PrimaryReviewers = new List<SubjectSet>()
                    {
                        new GroupMembers()
                        {
                            GroupId = approverGroupId
                        }
                    },
                    Schedule = new EntitlementManagementSchedule
                    {
                        StartDateTime = DateTime.Now,
                        Expiration = new ExpirationPattern
                        {
                            Duration = new Duration("P14D"),
                            Type = ExpirationPatternType.AfterDuration
                        },
                        Recurrence = new PatternedRecurrence
                        {
                            Pattern = new RecurrencePattern
                            {
                                Type = RecurrencePatternType.AbsoluteMonthly,
                                Interval = 1
                            },
                            Range = new RecurrenceRange
                            {
                                Type = RecurrenceRangeType.NoEnd,
                                StartDate = new Date(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
                            }
                        }
                    }
                },

                AccessPackage = new AccessPackage
                {
                    Id = accessPackageId
                }
            };

            var accessPackageAssignmentPolicyList = await GetAccessPackageAssignmentPolicyList(graphServiceClient);

            AccessPackageAssignmentPolicy assignmentPolicy = accessPackageAssignmentPolicyList.FirstOrDefault<AccessPackageAssignmentPolicy>(x => x.DisplayName == accessPackageAssignmentPolicy.DisplayName);
            if (assignmentPolicy is null)
            {
                AccessPackageAssignmentPolicy accessPackageAssignmentPolicyResult =  await graphServiceClient.IdentityGovernance.EntitlementManagement.AssignmentPolicies
                    .Request()
                    .AddAsync(accessPackageAssignmentPolicy);

                // Requires Beta API update to set accessRequests = true
                Console.WriteLine(string.Format("{0} access package assignment policy created.", accessPackageAssignmentPolicyResult.DisplayName));
            }
            else
            {
                throw new Exception(string.Format("ERROR: '{0}' access package assignment policy already exists.", accessPackageAssignmentPolicy.DisplayName));
            }
        }

        private static async Task<IEntitlementManagementAssignmentPoliciesCollectionPage> GetAccessPackageAssignmentPolicyList(GraphServiceClient graphServiceClient)
        {
            var accessPackagesPolicies = await graphServiceClient.IdentityGovernance.EntitlementManagement.AssignmentPolicies
                .Request()
                .GetAsync();

            return accessPackagesPolicies;
        }
    }
}
