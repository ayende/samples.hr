using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents;
using HRAssistant.Models;

namespace HRAssistant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeedController : ControllerBase
    {
        private readonly IDocumentStore _documentStore;
        private readonly ILogger<SeedController> _logger;

        public SeedController(IDocumentStore documentStore, ILogger<SeedController> logger)
        {
            _documentStore = documentStore;
            _logger = logger;
        }

        [HttpPost("all")]
        public async Task<ActionResult> SeedAllData()
        {
            using var session = _documentStore.OpenAsyncSession();

            try
            {
                // Create sample departments
                await SeedDepartments(session);

                // Create sample employees
                await SeedEmployees(session);

                // Create HR Policies
                await SeedHRPolicies(session);

                // Create vacation requests
                await SeedVacationRequests(session);

                // Create pay stubs
                await SeedPayStubs(session);

                // Create HR issues
                await SeedHRIssues(session);

                // Create signature documents
                await SeedSignatureDocuments(session);

                await session.SaveChangesAsync();
                return Ok(new { Message = "All sample data created successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error seeding data");
                return StatusCode(500, new { Message = "Error creating sample data", Error = ex.Message });
            }
        }

        private async Task SeedDepartments(Raven.Client.Documents.Session.IAsyncDocumentSession session)
        {
            var departments = new List<Department>
            {
                new() { Id = "departments/it", Name = "Information Technology", Manager = "Alice Johnson", ManagerId = "employees/alice", Building = "A", Floor = "3", Description = "Technology infrastructure and support", ResponsibleFor = new() { "IT", "Technology", "Software", "Hardware" } },
                new() { Id = "departments/hr", Name = "Human Resources", Manager = "Bob Smith", ManagerId = "employees/bob", Building = "B", Floor = "2", Description = "Employee relations and policies", ResponsibleFor = new() { "HR", "Policies", "Benefits", "Hiring" } },
                new() { Id = "departments/finance", Name = "Finance", Manager = "Carol Wilson", ManagerId = "employees/carol", Building = "C", Floor = "1", Description = "Financial planning and accounting", ResponsibleFor = new() { "Finance", "Accounting", "Budgets", "Expenses" } },
                new() { Id = "departments/marketing", Name = "Marketing", Manager = "David Brown", ManagerId = "employees/david", Building = "A", Floor = "2", Description = "Marketing and communications", ResponsibleFor = new() { "Marketing", "Communications", "Branding" } },
                new() { Id = "departments/engineering", Name = "Engineering", Manager = "John Dev", ManagerId = "employees/john", Building = "A", Floor = "4", Description = "Software development and engineering", ResponsibleFor = new() { "Development", "Engineering", "Architecture", "Quality Assurance" } }
            };

            foreach (var dept in departments)
            {
                await session.StoreAsync(dept);
            }
        }

        private async Task SeedEmployees(Raven.Client.Documents.Session.IAsyncDocumentSession session)
        {
            var employees = new List<Employee>
            {
                new()
                {
                    Id = "employees/alice",
                    Name = "Alice Johnson",
                    Department = "Information Technology",
                    DepartmentId = "departments/it",
                    EmploymentType = "full-time",
                    HireDate = new DateTime(2022, 3, 15),
                    CriticalRole = true,
                    JobTitle = "IT Manager",
                    Email = "alice.johnson@company.com",
                    Building = "A",
                    Vacation = new VacationInfo
                    {
                        AnnualEntitlement = 25,
                        AccruedDays = 18.5m,
                        CarryOverDays = 3,
                        Balance = 21.5m,
                        History = new() { new() { Year = 2025, UsedDays = 7, CarryOverUsed = 0, Requests = new() } }
                    }
                },
                new()
                {
                    Id = "employees/bob",
                    Name = "Bob Smith",
                    Department = "Human Resources",
                    DepartmentId = "departments/hr",
                    EmploymentType = "full-time",
                    HireDate = new DateTime(2021, 8, 1),
                    CriticalRole = true,
                    JobTitle = "HR Manager",
                    Email = "bob.smith@company.com",
                    Building = "B",
                    Vacation = new VacationInfo
                    {
                        AnnualEntitlement = 25,
                        AccruedDays = 20m,
                        CarryOverDays = 5,
                        Balance = 25m,
                        History = new() { new() { Year = 2025, UsedDays = 5, CarryOverUsed = 2, Requests = new() } }
                    }
                },
                new()
                {
                    Id = "employees/carol",
                    Name = "Carol Wilson",
                    Department = "Finance",
                    DepartmentId = "departments/finance",
                    EmploymentType = "full-time",
                    HireDate = new DateTime(2023, 1, 10),
                    CriticalRole = false,
                    JobTitle = "Finance Manager",
                    Email = "carol.wilson@company.com",
                    Building = "C",
                    Vacation = new VacationInfo
                    {
                        AnnualEntitlement = 20,
                        AccruedDays = 15m,
                        CarryOverDays = 0,
                        Balance = 15m,
                        History = new() { new() { Year = 2025, UsedDays = 8, CarryOverUsed = 0, Requests = new() } }
                    }
                },
                new()
                {
                    Id = "employees/david",
                    Name = "David Brown",
                    Department = "Marketing",
                    DepartmentId = "departments/marketing",
                    EmploymentType = "full-time",
                    HireDate = new DateTime(2024, 6, 1),
                    CriticalRole = false,
                    JobTitle = "Marketing Manager",
                    Email = "david.brown@company.com",
                    Building = "A",
                    Vacation = new VacationInfo
                    {
                        AnnualEntitlement = 20,
                        AccruedDays = 12m,
                        CarryOverDays = 0,
                        Balance = 12m,
                        History = new() { new() { Year = 2025, UsedDays = 3, CarryOverUsed = 0, Requests = new() } }
                    }
                },
                new()
                {
                    Id = "employees/john",
                    Name = "John Dev",
                    Department = "Engineering",
                    DepartmentId = "departments/engineering",
                    EmploymentType = "full-time",
                    HireDate = new DateTime(2024, 5, 1),
                    CriticalRole = true,
                    JobTitle = "Senior Developer",
                    Email = "john.Dev@company.com",
                    Building = "A",
                    Vacation = new VacationInfo
                    {
                        AnnualEntitlement = 20,
                        AccruedDays = 15m,
                        CarryOverDays = 5,
                        Balance = 15m,
                        History = new() { new() { Year = 2025, UsedDays = 5, CarryOverUsed = 0, Requests = new() } }
                    }
                },
                new()
                {
                    Id = "employees/sarah",
                    Name = "Sarah Martinez",
                    Department = "Engineering",
                    DepartmentId = "departments/engineering",
                    EmploymentType = "full-time",
                    HireDate = new DateTime(2023, 9, 15),
                    CriticalRole = false,
                    JobTitle = "Software Engineer",
                    Email = "sarah.martinez@company.com",
                    Building = "A",
                    Vacation = new VacationInfo
                    {
                        AnnualEntitlement = 20,
                        AccruedDays = 16m,
                        CarryOverDays = 2,
                        Balance = 18m,
                        History = new() { new() { Year = 2025, UsedDays = 4, CarryOverUsed = 0, Requests = new() } }
                    }
                },
                new()
                {
                    Id = "employees/mike",
                    Name = "Mike Thompson",
                    Department = "Information Technology",
                    DepartmentId = "departments/it",
                    EmploymentType = "full-time",
                    HireDate = new DateTime(2023, 11, 20),
                    CriticalRole = false,
                    JobTitle = "IT Support Specialist",
                    Email = "mike.thompson@company.com",
                    Building = "A",
                    Vacation = new VacationInfo
                    {
                        AnnualEntitlement = 15,
                        AccruedDays = 12m,
                        CarryOverDays = 0,
                        Balance = 12m,
                        History = new() { new() { Year = 2025, UsedDays = 6, CarryOverUsed = 0, Requests = new() } }
                    }
                }
            };

            foreach (var employee in employees)
            {
                await session.StoreAsync(employee);
            }
        }

        private async Task SeedHRPolicies(Raven.Client.Documents.Session.IAsyncDocumentSession session)
        {
            var policies = new List<HRPolicy>
            {
                new()
                {
                    Id = "policies/vacation-time",
                    Title = "Vacation Time and Leave Policy",
                    Category = "Time Off",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "HR Department",
                    Tags = new() { "vacation", "time off", "leave", "pto" },
                    Content = @"# Vacation Time and Leave Policy

## Purpose and Scope
This policy outlines the vacation time and leave entitlements for all full-time and part-time employees. The company recognizes the importance of work-life balance and provides competitive vacation benefits to ensure employee well-being and productivity. This policy applies to all regular employees who have completed their probationary period and are in good standing with the company.

## Vacation Accrual and Entitlement
Full-time employees accrue vacation time based on their length of service with the company. New employees with 0-2 years of service receive 15 days annually, employees with 2-5 years receive 20 days, and those with 5+ years receive 25 days. Vacation time accrues monthly at a rate of 1/12th of the annual entitlement. Part-time employees receive prorated vacation based on their scheduled hours compared to full-time equivalency.

## Usage and Approval Process
Vacation requests must be submitted through the HR system at least two weeks in advance for requests of 3+ days, and one week for shorter periods. Approval is subject to business needs and staffing requirements. During peak business periods, vacation requests may be limited. Employees are encouraged to plan vacation time throughout the year rather than taking extended periods that may impact operations.

## Carryover and Payout Provisions
Unused vacation time may be carried over to the following year up to a maximum of 5 days for employees with less than 5 years of service, and 10 days for those with 5+ years. Any vacation time exceeding the carryover limit will be forfeited unless special circumstances apply. Upon termination of employment, accrued but unused vacation time will be paid out at the employee's current rate of pay, subject to applicable laws and regulations."
                },
                new()
                {
                    Id = "policies/security-awareness",
                    Title = "Information Security Awareness Policy",
                    Category = "Security",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "IT Security Team",
                    Tags = new() { "security", "cybersecurity", "data protection", "compliance" },
                    Content = @"# Information Security Awareness Policy

## Policy Overview
Information security is everyone's responsibility. This policy establishes requirements for maintaining the confidentiality, integrity, and availability of company information assets. All employees, contractors, and third parties with access to company systems must comply with these security requirements to protect against data breaches, cyber attacks, and unauthorized access to sensitive information.

## Password and Authentication Requirements
All users must create strong passwords containing at least 12 characters with a combination of uppercase letters, lowercase letters, numbers, and special characters. Passwords must be unique and not reused for the past 12 iterations. Multi-factor authentication (MFA) is required for all business-critical systems including email, financial systems, and customer databases. Employees must not share passwords or authentication credentials under any circumstances.

## Data Handling and Classification
Company data is classified into three categories: Public, Internal, and Confidential. Public data may be freely shared, Internal data is for company use only, and Confidential data requires special handling and encryption. All confidential data must be encrypted both in transit and at rest. Employees must only access data necessary for their job functions and must not store sensitive data on personal devices or unauthorized cloud services.

## Incident Reporting and Response
Security incidents must be reported immediately to the IT Security team through the designated incident reporting system or by calling the security hotline. This includes suspected malware infections, phishing attempts, unauthorized access, data breaches, or loss of company devices. Employees must not attempt to investigate or resolve security incidents independently, as this may compromise evidence or worsen the situation. The IT Security team will coordinate the appropriate response and remediation efforts."
                },
                new()
                {
                    Id = "policies/equipment-request",
                    Title = "Equipment Request and Procurement Policy",
                    Category = "IT Equipment",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "Operations Department",
                    Tags = new() { "equipment", "procurement", "IT", "hardware", "software" },
                    Content = @"# Equipment Request and Procurement Policy

## Purpose and Approval Authority
This policy governs the process for requesting, approving, and procuring business equipment including computers, software, office furniture, and specialized tools. All equipment requests must be submitted through the approved procurement system and require appropriate management approval based on the total cost and business justification. Requests under $500 require supervisor approval, $500-$2,500 require department manager approval, and over $2,500 require director-level approval.

## Request Process and Documentation
Employees must submit equipment requests using the online procurement portal, providing detailed specifications, business justification, and preferred vendors when applicable. Requests must include the intended use, expected lifespan, and how the equipment will improve productivity or meet business requirements. All requests are reviewed by the IT department for compatibility and security compliance before procurement approval. Emergency requests may be expedited but still require proper documentation and approval.

## Budget Planning and Asset Management
Equipment purchases must align with departmental budgets and annual planning cycles. Major equipment purchases should be included in the annual budget planning process to ensure adequate funding allocation. All purchased equipment becomes company property and must be properly tagged and recorded in the asset management system. Employees are responsible for the care and security of assigned equipment and must report any damage, loss, or theft immediately.

## Return and Disposal Procedures
Upon termination or role changes, employees must return all company equipment in good working condition. The IT department will coordinate equipment retrieval and perform security wipes of all data storage devices. Disposal of outdated or damaged equipment must follow environmental regulations and data security requirements. Personal use of company equipment is prohibited, and any software installations must be approved by the IT department to maintain security and licensing compliance."
                },
                new()
                {
                    Id = "policies/allergy-awareness",
                    Title = "Workplace Allergy Awareness and Accommodation Policy",
                    Category = "Health & Safety",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "Human Resources",
                    Tags = new() { "allergies", "health", "safety", "accommodation", "workplace" },
                    Content = @"# Workplace Allergy Awareness and Accommodation Policy

## Policy Statement and Commitment
The company is committed to providing a safe and inclusive workplace for all employees, including those with food allergies, environmental allergies, and chemical sensitivities. We recognize that allergic reactions can range from mild discomfort to life-threatening anaphylaxis, and we will work collaboratively with affected employees to implement reasonable accommodations that protect their health while maintaining a productive work environment for all team members.

## Disclosure and Accommodation Process
Employees with allergies are encouraged to disclose their conditions to Human Resources to facilitate appropriate workplace accommodations. While disclosure is voluntary, it enables the company to take proactive measures to ensure employee safety. The accommodation process involves a confidential discussion between the employee, HR, and relevant managers to identify potential triggers in the workplace and develop an individualized accommodation plan that addresses the specific needs while considering operational requirements.

## Workplace Modifications and Protocols
Common accommodations may include designating allergen-free zones, modifying cleaning products and air fresheners, implementing fragrance-free policies in specific areas, or adjusting food policies for shared spaces. The company will provide training to managers and team members on allergy awareness when necessary, while respecting the privacy of affected employees. Emergency action plans will be developed for employees with severe allergies, including the location of epinephrine auto-injectors and emergency contact procedures.

## Shared Responsibility and Communication
Creating an allergy-conscious workplace requires cooperation from all employees. Team members are asked to be mindful of common allergens in shared spaces, participate in voluntary fragrance-reduction efforts when requested, and respect food restrictions in designated areas. The company will provide clear communication about any workplace policies related to allergen management, and employees are encouraged to communicate openly about concerns or suggestions for improving the workplace environment for everyone."
                },
                new()
                {
                    Id = "policies/remote-work",
                    Title = "Remote Work and Telecommuting Policy",
                    Category = "Work Arrangements",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "Human Resources",
                    Tags = new() { "remote work", "telecommuting", "flexible work", "work from home" },
                    Content = @"# Remote Work and Telecommuting Policy

## Eligibility and Approval Criteria
Remote work arrangements are available to eligible employees based on job responsibilities, performance history, and business needs. Positions suitable for remote work typically involve tasks that can be completed independently with minimal in-person collaboration requirements. Employees must have a track record of meeting performance expectations, demonstrate self-motivation and time management skills, and have been employed with the company for at least six months before being eligible for regular remote work arrangements.

## Work Environment and Equipment Standards
Remote workers are responsible for establishing a professional, safe, and productive home office environment. The workspace should be free from distractions, have adequate lighting and ventilation, and comply with basic ergonomic principles. The company will provide necessary technology equipment including laptops, monitors, and communication tools, while employees are responsible for reliable internet connectivity and basic office supplies. All company equipment remains company property and must be returned upon request or termination.

## Communication and Collaboration Requirements
Remote employees must maintain regular communication with their supervisors and team members through approved channels including email, video conferencing, and project management tools. Core collaboration hours may be established to ensure overlap with office-based colleagues and client needs. Remote workers are expected to participate in all required meetings, training sessions, and team activities through virtual means. Response time expectations for emails and messages remain the same as office-based employees.

## Performance Management and Evaluation
Remote work success is measured by results and outcomes rather than hours worked. Employees and supervisors will establish clear goals, deadlines, and performance metrics to ensure accountability and productivity. Regular check-ins and performance reviews will assess both work quality and remote work effectiveness. The company reserves the right to modify or terminate remote work arrangements if performance standards are not met, business needs change, or if the arrangement is not working effectively for the team or organization."
                },
                new()
                {
                    Id = "policies/code-of-conduct",
                    Title = "Employee Code of Conduct and Ethics Policy",
                    Category = "Ethics",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "HR Department",
                    Tags = new() { "ethics", "conduct", "behavior", "compliance" },
                    Content = @"# Employee Code of Conduct and Ethics Policy

## Core Values and Principles
This code of conduct establishes the ethical standards and behavioral expectations for all employees, contractors, and representatives of the company. Our core values of integrity, respect, accountability, and excellence guide all business decisions and interactions. Every employee is expected to act honestly, treat others with dignity, take responsibility for their actions, and strive for the highest quality in their work while contributing to a positive and inclusive workplace culture.

## Professional Behavior Standards
Employees must maintain professionalism in all business interactions, whether with colleagues, customers, vendors, or the public. This includes communicating respectfully, dressing appropriately for the work environment, arriving punctually for meetings and commitments, and representing the company's values in all business activities. Discrimination, harassment, bullying, or any form of hostile behavior will not be tolerated and may result in disciplinary action up to and including termination.

## Conflicts of Interest and Business Ethics
Employees must avoid conflicts of interest and situations where personal interests could compromise professional judgment or company interests. This includes financial interests in competitors, vendors, or customers, as well as outside employment that creates competing obligations. Any potential conflicts must be disclosed to management immediately. Employees must not accept gifts, entertainment, or other benefits from business partners that could influence business decisions or create the appearance of impropriety.

## Confidentiality and Reporting Violations
All employees have a duty to protect confidential company information, including customer data, financial information, trade secrets, and strategic plans. Confidential information must not be shared with unauthorized parties or used for personal gain. Employees who become aware of violations of this code of conduct, company policies, or applicable laws have a responsibility to report these concerns through appropriate channels including their supervisor, Human Resources, or the anonymous ethics hotline. The company prohibits retaliation against employees who report concerns in good faith."
                },
                new()
                {
                    Id = "policies/attendance-punctuality",
                    Title = "Attendance and Punctuality Policy",
                    Category = "Time & Attendance",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "HR Department",
                    Tags = new() { "attendance", "punctuality", "time", "schedules" },
                    Content = @"# Attendance and Punctuality Policy

## Expectations and Schedule Requirements
Regular attendance and punctuality are essential for maintaining productive operations and team collaboration. All employees are expected to report to work as scheduled and remain for their entire shift unless authorized to leave early. Core business hours are 9:00 AM to 5:00 PM, Monday through Friday, with specific departments having variations based on operational needs. Employees must be present and ready to work at their scheduled start time, not merely arriving at that time.

## Time Tracking and Documentation
All employees must accurately record their time using the approved time tracking system, including start times, end times, break periods, and any time away from work. Time records must be submitted by the specified deadline each pay period and reviewed for accuracy before submission. Falsification of time records is considered a serious violation and may result in disciplinary action. Supervisors are responsible for reviewing and approving time records and addressing any discrepancies promptly.

## Absence Notification and Approval
Planned absences must be requested in advance through the appropriate channels and approved by the immediate supervisor. For unplanned absences due to illness or emergency, employees must notify their supervisor as soon as possible, preferably before their scheduled start time. Failure to provide adequate notice may result in the absence being considered unexcused. Consecutive absences of three or more days may require medical documentation upon return to work.

## Progressive Discipline and Corrective Action
Chronic attendance problems or excessive unexcused absences will result in progressive disciplinary action. The typical progression includes verbal counseling, written warning, final written warning, and termination, though severe cases may warrant more immediate action. Factors considered include the frequency and pattern of absences, impact on operations, employee's overall performance record, and any mitigating circumstances. Employees experiencing attendance difficulties are encouraged to speak with Human Resources about available resources and support options."
                },
                new()
                {
                    Id = "policies/professional-development",
                    Title = "Professional Development and Training Policy",
                    Category = "Learning & Development",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "Learning & Development Team",
                    Tags = new() { "training", "development", "education", "skills", "career growth" },
                    Content = @"# Professional Development and Training Policy

## Commitment to Employee Growth
The company is committed to supporting employee professional development through various training opportunities, educational assistance, and career advancement programs. We believe that investing in our employees' skills and knowledge benefits both individual career growth and organizational success. All employees are encouraged to participate in development activities that enhance their current role performance and prepare them for future opportunities within the company.

## Training Programs and Opportunities
The company offers a variety of development opportunities including internal training programs, external workshops and seminars, online learning platforms, professional conferences, and industry certifications. Mandatory training programs cover areas such as safety, compliance, and job-specific skills, while optional programs focus on leadership development, technical skills, and personal effectiveness. Employees should work with their supervisors to identify relevant training opportunities that align with their role requirements and career goals.

## Educational Assistance and Reimbursement
Eligible employees may receive financial assistance for job-related education including college courses, professional certifications, and industry training programs. Reimbursement is available for approved educational expenses up to the annual limit specified in the benefits program. Employees must receive pre-approval for educational assistance and maintain satisfactory progress to continue receiving support. Recipients of educational assistance may be required to remain employed with the company for a specified period following completion of the program.

## Performance Integration and Career Planning
Professional development activities should be integrated into the annual performance review process, with employees and supervisors collaborating to identify skill gaps, development goals, and appropriate learning opportunities. Individual development plans will be created to outline specific objectives, timelines, and success measures. The company supports internal career advancement and encourages employees to apply for open positions that match their skills and interests, with preference given to qualified internal candidates who have demonstrated strong performance and development initiative."
                },
                new()
                {
                    Id = "policies/social-media",
                    Title = "Social Media and Online Communication Policy",
                    Category = "Communications",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "Marketing & HR Departments",
                    Tags = new() { "social media", "online", "communication", "digital presence" },
                    Content = @"# Social Media and Online Communication Policy

## Personal vs. Professional Social Media Use
This policy provides guidelines for appropriate social media use both during work hours and when representing the company online. Employees are free to use personal social media accounts as private individuals, but must clearly distinguish personal opinions from company positions. When company affiliation is mentioned in social media profiles, employees should include a disclaimer that views expressed are their own and not those of the company. Personal social media activity should not interfere with work performance or productivity.

## Company Representation and Brand Protection
Employees who are authorized to represent the company on social media must follow established brand guidelines and communication protocols. All company-related social media content should be professional, accurate, and consistent with company values and messaging. Confidential company information, including financial data, strategic plans, customer information, and unreleased products or services, must never be shared on social media platforms. Employees should think carefully before posting content that could reflect negatively on the company's reputation.

## Professional Conduct Online
The same standards of professional conduct that apply in the workplace extend to online interactions. Harassment, discrimination, threats, or inappropriate content targeting colleagues, customers, competitors, or business partners is prohibited and may result in disciplinary action. Employees should be respectful in online discussions and avoid engaging in inflammatory debates or controversial topics that could damage professional relationships or the company's reputation.

## Compliance and Monitoring
Employees in certain roles may be subject to industry regulations regarding social media use and disclosure requirements. All employees must comply with applicable laws and regulations when using social media, including advertising standards, intellectual property rights, and privacy requirements. While the company respects employee privacy, social media content that violates company policies or creates legal liability may result in disciplinary action. Employees are encouraged to use privacy settings appropriately and consider the public nature of social media when posting content."
                },
                new()
                {
                    Id = "policies/workplace-safety",
                    Title = "Workplace Safety and Emergency Procedures Policy",
                    Category = "Health & Safety",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "Safety Committee",
                    Tags = new() { "safety", "emergency", "procedures", "health", "workplace" },
                    Content = @"# Workplace Safety and Emergency Procedures Policy

## Safety Commitment and Responsibilities
The company is committed to providing a safe and healthy work environment for all employees, visitors, and contractors. Safety is everyone's responsibility, and all individuals in the workplace are expected to follow safety procedures, report hazards, and participate in safety training programs. Management is responsible for providing necessary safety equipment, maintaining safe working conditions, and ensuring compliance with all applicable health and safety regulations. Employees have the right to refuse unsafe work and the responsibility to work safely.

## Hazard Identification and Reporting
All employees must immediately report safety hazards, accidents, injuries, and near-miss incidents to their supervisor and the safety coordinator. This includes physical hazards such as spills, damaged equipment, or blocked emergency exits, as well as ergonomic concerns and environmental issues. The company maintains an incident reporting system that allows for anonymous reporting of safety concerns. All reported hazards will be investigated promptly, and corrective action will be taken to prevent recurrence.

## Emergency Procedures and Evacuation Plans
Detailed emergency procedures are posted throughout the workplace and cover various scenarios including fire, medical emergencies, severe weather, and security threats. All employees must familiarize themselves with evacuation routes, assembly points, and emergency contact procedures. Regular emergency drills will be conducted to ensure preparedness and identify areas for improvement. Emergency response team members receive specialized training and are responsible for assisting with evacuations and emergency response coordination.

## Personal Protective Equipment and Training
The company provides necessary personal protective equipment (PPE) at no cost to employees whose job responsibilities require its use. Employees must use PPE as directed, maintain equipment in good condition, and report any damaged or defective equipment immediately. Comprehensive safety training is provided to all new employees and updated regularly to address changing conditions and requirements. Additional specialized training is provided for employees working with hazardous materials, operating machinery, or performing high-risk tasks."
                },
                new()
                {
                    Id = "policies/data-privacy",
                    Title = "Data Privacy and Protection Policy",
                    Category = "Privacy & Compliance",
                    LastUpdated = DateTime.Now,
                    UpdatedBy = "Legal & Compliance Team",
                    Tags = new() { "data privacy", "protection", "GDPR", "compliance", "personal data" },
                    Content = @"# Data Privacy and Protection Policy

## Privacy Principles and Legal Compliance
This policy establishes requirements for protecting personal data and ensuring compliance with applicable privacy laws including GDPR, CCPA, and other regional privacy regulations. The company is committed to respecting individual privacy rights and implementing appropriate technical and organizational measures to protect personal data from unauthorized access, use, disclosure, or destruction. All employees who handle personal data must understand their responsibilities and comply with privacy requirements in their daily work activities.

## Data Collection and Processing Principles
Personal data must only be collected and processed for legitimate business purposes with appropriate legal basis such as consent, contract performance, or legitimate interests. Data collection should be limited to what is necessary for the intended purpose, and individuals should be informed about how their data will be used through clear and transparent privacy notices. Data accuracy must be maintained, and individuals have rights to access, correct, delete, or restrict processing of their personal data subject to legal and business requirements.

## Data Security and Access Controls
All personal data must be protected through appropriate security measures including encryption, access controls, regular security assessments, and employee training. Access to personal data should be limited to employees who need it for their job responsibilities, and access should be regularly reviewed and updated. Data transfers to third parties or across borders must comply with applicable legal requirements and include appropriate contractual protections. Regular security monitoring and incident response procedures help detect and respond to potential data breaches.

## Breach Response and Individual Rights
Any suspected or confirmed data breach must be reported immediately to the Data Protection Officer and may require notification to regulatory authorities and affected individuals within specified timeframes. The company maintains procedures for responding to individual requests regarding their personal data, including access requests, correction requests, and deletion requests. Employees must cooperate with privacy compliance efforts and report any concerns about data handling practices. Regular privacy training and awareness programs ensure ongoing compliance with evolving privacy requirements."
                }
            };

            foreach (var policy in policies)
            {
                await session.StoreAsync(policy);
            }
        }

        private async Task SeedVacationRequests(Raven.Client.Documents.Session.IAsyncDocumentSession session)
        {
            var vacationRequests = new List<VacationRequest>
            {
                new()
                {
                    Id = "vacations/alice-2025-001",
                    EmployeeId = "employees/alice",
                    EmployeeName = "Alice Johnson",
                    StartDate = new DateTime(2025, 3, 15),
                    EndDate = new DateTime(2025, 3, 19),
                    DaysRequested = 5,
                    Reason = "Family vacation to Hawaii",
                    Status = "Approved",
                    SubmittedDate = new DateTime(2025, 2, 1),
                    ApprovedDate = new DateTime(2025, 2, 3),
                    ApprovedBy = "employees/bob",
                    ReplacementEmployee = new Replacement
                    {
                        EmployeeId = "employees/mike",
                        Name = "Mike Thompson"
                    },
                    VacationType = "Annual Leave"
                },
                new()
                {
                    Id = "vacations/john-2025-001",
                    EmployeeId = "employees/john",
                    EmployeeName = "John Dev",
                    StartDate = new DateTime(2025, 4, 22),
                    EndDate = new DateTime(2025, 4, 26),
                    DaysRequested = 5,
                    Reason = "Spring break with family",
                    Status = "Pending",
                    SubmittedDate = new DateTime(2025, 3, 10),
                    ApprovedDate = null,
                    ApprovedBy = "employees/alice",
                    ReplacementEmployee = new Replacement
                    {
                        EmployeeId = "employees/sarah",
                        Name = "Sarah Martinez"
                    },
                    VacationType = "Annual Leave"
                },
                new()
                {
                    Id = "vacations/sarah-2025-001",
                    EmployeeId = "employees/sarah",
                    EmployeeName = "Sarah Martinez",
                    StartDate = new DateTime(2025, 5, 1),
                    EndDate = new DateTime(2025, 5, 1),
                    DaysRequested = 1,
                    Reason = "Personal appointment",
                    Status = "Approved",
                    SubmittedDate = new DateTime(2025, 4, 15),
                    ApprovedDate = new DateTime(2025, 4, 16),
                    ApprovedBy = "employees/alice",
                    VacationType = "Personal Day"
                },
                new()
                {
                    Id = "vacations/bob-2025-001",
                    EmployeeId = "employees/bob",
                    EmployeeName = "Bob Smith",
                    StartDate = new DateTime(2025, 6, 10),
                    EndDate = new DateTime(2025, 6, 21),
                    DaysRequested = 10,
                    Reason = "European vacation",
                    Status = "Pending",
                    SubmittedDate = new DateTime(2025, 4, 1),
                    VacationType = "Annual Leave"
                },
                new()
                {
                    Id = "vacations/carol-2025-001",
                    EmployeeId = "employees/carol",
                    EmployeeName = "Carol Wilson",
                    StartDate = new DateTime(2025, 7, 4),
                    EndDate = new DateTime(2025, 7, 4),
                    DaysRequested = 1,
                    Reason = "Independence Day extended weekend",
                    Status = "Approved",
                    SubmittedDate = new DateTime(2025, 5, 1),
                    ApprovedDate = new DateTime(2025, 5, 2),
                    ApprovedBy = "employees/bob",
                    VacationType = "Personal Day"
                }
            };

            foreach (var vacation in vacationRequests)
            {
                await session.StoreAsync(vacation);
            }
        }

        private async Task SeedPayStubs(Raven.Client.Documents.Session.IAsyncDocumentSession session)
        {
            var employees = new[]
            {
                new {
                    Id = "alice",
                    Name = "Alice Johnson",
                    MonthlySalary = 8333.33m,
                    BankName = "Wells Fargo",
                    RoutingNumber = "121000248",
                    AccountNumber = "****4567"
                },
                new {
                    Id = "bob",
                    Name = "Bob Smith",
                    MonthlySalary = 7500.00m,
                    BankName = "Chase Bank",
                    RoutingNumber = "021000021",
                    AccountNumber = "****8901"
                },
                new {
                    Id = "carol",
                    Name = "Carol Wilson",
                    MonthlySalary = 7000.00m,
                    BankName = "Bank of America",
                    RoutingNumber = "026009593",
                    AccountNumber = "****2345"
                },
                new {
                    Id = "david",
                    Name = "David Brown",
                    MonthlySalary = 6500.00m,
                    BankName = "US Bank",
                    RoutingNumber = "091000019",
                    AccountNumber = "****6789"
                },
                new {
                    Id = "john",
                    Name = "John Dev",
                    MonthlySalary = 7500.00m,
                    BankName = "Capital One",
                    RoutingNumber = "051405515",
                    AccountNumber = "****1234"
                },
                new {
                    Id = "sarah",
                    Name = "Sarah Martinez",
                    MonthlySalary = 6000.00m,
                    BankName = "TD Bank",
                    RoutingNumber = "211274450",
                    AccountNumber = "****5678"
                },
                new {
                    Id = "mike",
                    Name = "Mike Thompson",
                    MonthlySalary = 4500.00m,
                    BankName = "PNC Bank",
                    RoutingNumber = "043000096",
                    AccountNumber = "****9012"
                }
            };

            var payStubs = new List<PayStub>();

            foreach (var emp in employees)
            {
                var ytdGross = 0m;
                var ytdNet = 0m;

                // Generate 6 months of pay stubs
                for (int month = 1; month <= 6; month++)
                {
                    var grossPay = emp.MonthlySalary;
                    var federalTax = grossPay * 0.22m;
                    var stateTax = grossPay * 0.05m;
                    var socialSecurity = grossPay * 0.062m;
                    var medicare = grossPay * 0.0145m;
                    var healthInsurance = 250m;
                    var retirement401k = grossPay * 0.06m;

                    var totalDeductions = federalTax + stateTax + socialSecurity + medicare + healthInsurance + retirement401k;
                    var netPay = grossPay - totalDeductions;

                    ytdGross += grossPay;
                    ytdNet += netPay;

                    var payStub = new PayStub
                    {
                        Id = $"paystubs/{emp.Id}-2025-{month:D2}",
                        EmployeeId = $"employees/{emp.Id}",
                        EmployeeName = emp.Name,
                        PayPeriodStart = new DateTime(2025, month, 1),
                        PayPeriodEnd = new DateTime(2025, month, DateTime.DaysInMonth(2025, month)),
                        PayDate = new DateTime(2025, month, DateTime.DaysInMonth(2025, month)),
                        GrossPay = grossPay,
                        NetPay = netPay,
                        YearToDateGross = ytdGross,
                        YearToDateNet = ytdNet,
                        PayPeriodNumber = month,
                        PayFrequency = "Monthly",
                        DirectDeposit = new ACHBankDetails
                        {
                            BankName = emp.BankName,
                            RoutingNumber = emp.RoutingNumber,
                            AccountNumber = emp.AccountNumber,
                            AccountType = "Checking",
                            DepositAmount = netPay,
                            DepositType = "Full Amount"
                        },
                        Earnings = new List<PayStubEarning>
                        {
                            new() { Type = "Base Salary", Hours = 173.33m, Rate = grossPay / 173.33m, Amount = grossPay, YearToDate = ytdGross }
                        },
                        Deductions = new List<PayStubDeduction>
                        {
                            new() { Type = "Health Insurance", Amount = healthInsurance, YearToDate = healthInsurance * month, PreTax = true },
                            new() { Type = "401(k) Contribution", Amount = retirement401k, YearToDate = retirement401k * month, PreTax = true }
                        },
                        Taxes = new List<PayStubTax>
                        {
                            new() { Type = "Federal Income Tax", Amount = federalTax, YearToDate = federalTax * month, TaxableWages = grossPay },
                            new() { Type = "State Income Tax", Amount = stateTax, YearToDate = stateTax * month, TaxableWages = grossPay },
                            new() { Type = "Social Security", Amount = socialSecurity, YearToDate = socialSecurity * month, TaxableWages = grossPay },
                            new() { Type = "Medicare", Amount = medicare, YearToDate = medicare * month, TaxableWages = grossPay }
                        }
                    };

                    payStubs.Add(payStub);
                }
            }

            foreach (var payStub in payStubs)
            {
                await session.StoreAsync(payStub);
            }
        }

        private async Task SeedHRIssues(Raven.Client.Documents.Session.IAsyncDocumentSession session)
        {
            var issues = new List<HRIssue>
            {
                new()
                {
                    Id = "issues/sarah-001",
                    EmployeeId = "employees/sarah",
                    EmployeeName = "Sarah Martinez",
                    Title = "Health Insurance Coverage Question",
                    Description = "I need clarification on whether my spouse can be added to my health insurance plan mid-year due to their job loss. They lost coverage on March 1st and I want to make sure I follow the correct enrollment process.",
                    Category = "Benefits",
                    Priority = "Medium",
                    Status = "Resolved",
                    SubmittedDate = new DateTime(2025, 3, 5),
                    AssignedDate = new DateTime(2025, 3, 5),
                    ResolvedDate = new DateTime(2025, 3, 8),
                    AssignedTo = "employees/bob",
                    Resolution = "Loss of spouse's coverage qualifies as a life event allowing mid-year enrollment. Provided enrollment forms and deadline information.",
                    Tags = new() { "benefits", "health insurance", "life event" },
                    Comments = new List<HRIssueComment>
                    {
                        new() { Id = "comment-001", AuthorId = "employees/bob", AuthorName = "Bob Smith", Content = "This qualifies as a qualifying life event. I'll send you the enrollment forms.", CreatedDate = new DateTime(2025, 3, 6) },
                        new() { Id = "comment-002", AuthorId = "employees/sarah", AuthorName = "Sarah Martinez", Content = "Thank you! Forms submitted.", CreatedDate = new DateTime(2025, 3, 8) }
                    }
                },
                new()
                {
                    Id = "issues/mike-001",
                    EmployeeId = "employees/mike",
                    EmployeeName = "Mike Thompson",
                    Title = "Request for New Monitor and Keyboard",
                    Description = "My current monitor is 19 inches and quite old, making it difficult to work efficiently with multiple applications. I would like to request a larger monitor (24-27 inches) and an ergonomic keyboard to improve my productivity and reduce eye strain.",
                    Category = "IT Request",
                    Priority = "Low",
                    Status = "Approved",
                    SubmittedDate = new DateTime(2025, 2, 15),
                    AssignedDate = new DateTime(2025, 2, 16),
                    ResolvedDate = new DateTime(2025, 2, 22),
                    AssignedTo = "employees/alice",
                    Resolution = "Approved 27-inch monitor and ergonomic keyboard. Equipment ordered and delivered.",
                    Tags = new() { "equipment", "monitor", "ergonomics" },
                    Comments = new List<HRIssueComment>
                    {
                        new() { Id = "comment-003", AuthorId = "employees/alice", AuthorName = "Alice Johnson", Content = "Request approved. Will order Dell 27-inch monitor and Microsoft ergonomic keyboard.", CreatedDate = new DateTime(2025, 2, 17) },
                        new() { Id = "comment-004", AuthorId = "employees/mike", AuthorName = "Mike Thompson", Content = "Equipment received and set up. Much better productivity!", CreatedDate = new DateTime(2025, 2, 23) }
                    }
                },
                new()
                {
                    Id = "issues/john-001",
                    EmployeeId = "employees/john",
                    EmployeeName = "John Dev",
                    Title = "Payroll Discrepancy - Missing Overtime Hours",
                    Description = "I noticed that my last paycheck did not include 4 hours of overtime that I worked on February 28th. I stayed late to complete the deployment and logged the time in the system, but it Devsn't appear on my pay stub.",
                    Category = "Payroll",
                    Priority = "High",
                    Status = "In Progress",
                    SubmittedDate = new DateTime(2025, 3, 10),
                    AssignedDate = new DateTime(2025, 3, 10),
                    AssignedTo = "employees/carol",
                    Tags = new() { "payroll", "overtime", "discrepancy" },
                    Comments = new List<HRIssueComment>
                    {
                        new() { Id = "comment-005", AuthorId = "employees/carol", AuthorName = "Carol Wilson", Content = "I'm reviewing the time records and will check with payroll processing. Will update you by end of week.", CreatedDate = new DateTime(2025, 3, 11), IsInternal = false },
                        new() { Id = "comment-006", AuthorId = "employees/carol", AuthorName = "Carol Wilson", Content = "Found the issue - overtime approval was not processed in time for payroll cutoff. Will include in next pay period.", CreatedDate = new DateTime(2025, 3, 12), IsInternal = true }
                    }
                },
                new()
                {
                    Id = "issues/david-001",
                    EmployeeId = "employees/david",
                    EmployeeName = "David Brown",
                    Title = "Remote Work Policy Clarification",
                    Description = "I would like to understand the company's policy on working remotely 2-3 days per week. My role involves mostly digital marketing activities that can be done from anywhere, and I believe this would improve my work-life balance while maintaining productivity.",
                    Category = "Policy Question",
                    Priority = "Medium",
                    Status = "Open",
                    SubmittedDate = new DateTime(2025, 3, 18),
                    AssignedDate = new DateTime(2025, 3, 19),
                    AssignedTo = "employees/bob",
                    Tags = new() { "remote work", "policy", "flexible work" },
                    Comments = new List<HRIssueComment>
                    {
                        new() { Id = "comment-007", AuthorId = "employees/bob", AuthorName = "Bob Smith", Content = "I'll review your role requirements and discuss with your manager to evaluate remote work suitability.", CreatedDate = new DateTime(2025, 3, 19) }
                    }
                },
                new()
                {
                    Id = "issues/alice-001",
                    EmployeeId = "employees/alice",
                    EmployeeName = "Alice Johnson",
                    Title = "Professional Development Conference Request",
                    Description = "I would like to attend the Cloud Security Summit in Seattle from May 15-17. This conference directly relates to our upcoming security initiatives and would provide valuable knowledge for our team. Registration is $1,200 plus travel expenses.",
                    Category = "Training Request",
                    Priority = "Medium",
                    Status = "Pending",
                    SubmittedDate = new DateTime(2025, 3, 25),
                    Tags = new() { "training", "conference", "professional development" },
                    Comments = new List<HRIssueComment>()
                },
                new()
                {
                    Id = "issues/carol-001",
                    EmployeeId = "employees/carol",
                    EmployeeName = "Carol Wilson",
                    Title = "Office Temperature Control Issue",
                    Description = "The temperature in the Finance department area (Building C, Floor 1) has been consistently too cold over the past two weeks. Several team members have complained, and it's affecting our comfort and productivity. The thermostat reads 65°F but it feels much colder.",
                    Category = "Facilities",
                    Priority = "Low",
                    Status = "Open",
                    SubmittedDate = new DateTime(2025, 3, 20),
                    Tags = new() { "facilities", "temperature", "comfort" },
                    Comments = new List<HRIssueComment>()
                }
            };

            foreach (var issue in issues)
            {
                await session.StoreAsync(issue);
            }
        }

        private async Task SeedSignatureDocuments(Raven.Client.Documents.Session.IAsyncDocumentSession session)
        {
            var documents = new List<SignatureDocument>
            {
                new()
                {
                    Id = "signature-documents/nda-standard",
                    Title = "Non-Disclosure Agreement (Standard)",
                    Type = "NDA",
                    Version = 2,
                    CreatedDate = new DateTime(2024, 1, 15),
                    CreatedBy = "Legal Department",
                    LastUpdated = new DateTime(2024, 6, 10),
                    UpdatedBy = "Legal Department",
                    Description = "Standard non-disclosure agreement for all employees handling confidential information",
                    Tags = new() { "confidentiality", "legal", "mandatory" },
                    ExpirationDays = 365, // Expires after 1 year
                    Content = @"# Non-Disclosure Agreement

## Confidentiality Commitment
By signing this agreement, I acknowledge that during my employment, I may have access to confidential information including but not limited to:
- Customer data and business relationships
- Proprietary software and technical specifications  
- Financial information and business strategies
- Personnel records and compensation data
- Marketing plans and competitive intelligence

## Obligations
I agree to:
1. Keep all confidential information strictly confidential
2. Not disclose any confidential information to unauthorized parties
3. Use confidential information solely for business purposes
4. Return all confidential materials upon termination
5. Report any suspected breaches immediately

This agreement remains in effect during employment and for 2 years after termination."
                },
                new()
                {
                    Id = "signature-documents/code-of-conduct",
                    Title = "Employee Code of Conduct",
                    Type = "Policy",
                    Version = 3,
                    CreatedDate = new DateTime(2024, 2, 1),
                    CreatedBy = "Human Resources",
                    LastUpdated = new DateTime(2024, 8, 1),
                    UpdatedBy = "Human Resources",
                    Description = "Comprehensive code of conduct covering workplace behavior and ethical standards",
                    Tags = new() { "ethics", "conduct", "workplace", "mandatory" },
                    Content = @"# Employee Code of Conduct

## Professional Standards
All employees are expected to maintain the highest standards of professional conduct, including:
- Treating colleagues, customers, and partners with respect and dignity
- Maintaining honesty and integrity in all business dealings
- Following all company policies and procedures
- Reporting violations or concerns through appropriate channels

## Prohibited Conduct
The following behaviors are strictly prohibited:
- Harassment or discrimination of any kind
- Conflicts of interest without proper disclosure
- Misuse of company resources or information
- Violation of safety protocols
- Substance abuse on company premises

## Compliance and Reporting
Employees must complete annual ethics training and report any policy violations promptly."
                },
                new()
                {
                    Id = "signature-documents/remote-work-agreement",
                    Title = "Remote Work Policy Acknowledgment",
                    Type = "Agreement",
                    Version = 1,
                    CreatedDate = new DateTime(2024, 3, 1),
                    CreatedBy = "Human Resources",
                    Description = "Acknowledgment of remote work policies and responsibilities",
                    Tags = new() { "remote work", "policy", "flexible work" },
                    Content = @"# Remote Work Policy Acknowledgment

## Work Environment Requirements
I acknowledge my responsibility to:
- Maintain a professional, safe, and productive home office
- Ensure reliable internet connectivity and communication access
- Protect company equipment and data in remote locations
- Maintain regular communication with supervisors and team members

## Performance and Accountability
I understand that remote work success is measured by results and that I must:
- Meet all performance expectations and deadlines
- Participate in required meetings and training sessions
- Be available during agreed-upon core collaboration hours
- Use company-approved tools and software for all work activities

## Compliance
I agree to follow all company policies while working remotely and understand that this arrangement may be modified or terminated based on business needs or performance."
                },
                new()
                {
                    Id = "signature-documents/safety-training",
                    Title = "Workplace Safety Training Completion",
                    Type = "Certification",
                    Version = 1,
                    CreatedDate = new DateTime(2024, 1, 10),
                    CreatedBy = "Safety Department",
                    Description = "Certification of completion of mandatory workplace safety training",
                    Tags = new() { "safety", "training", "certification", "mandatory" },
                    ExpirationDays = 365, // Annual renewal required
                    Content = @"# Workplace Safety Training Completion

## Training Completion Acknowledgment
I certify that I have completed the required workplace safety training covering:
- Emergency evacuation procedures and assembly points
- Fire safety and proper use of fire extinguishers
- First aid basics and location of emergency supplies
- Incident reporting procedures and safety protocols
- Equipment safety guidelines and PPE requirements

## Commitment to Safety
I commit to:
- Following all safety protocols and procedures
- Reporting hazards and incidents immediately
- Participating in emergency drills and safety meetings
- Using provided safety equipment properly
- Seeking clarification when unsure about safety procedures

## Annual Renewal
I understand this certification expires annually and requires refresher training."
                },
                new()
                {
                    Id = "signature-documents/data-privacy-policy",
                    Title = "Data Privacy and Protection Policy",
                    Type = "Policy",
                    Version = 2,
                    CreatedDate = new DateTime(2024, 4, 1),
                    CreatedBy = "IT Security",
                    LastUpdated = new DateTime(2024, 7, 15),
                    UpdatedBy = "IT Security",
                    Description = "Acknowledgment of data privacy responsibilities and GDPR compliance",
                    Tags = new() { "privacy", "data protection", "GDPR", "security" },
                    Content = @"# Data Privacy and Protection Policy

## Data Handling Responsibilities
I acknowledge my responsibility to protect personal data and understand that I must:
- Process personal data only for legitimate business purposes
- Implement appropriate security measures for data protection
- Report data breaches or suspected breaches immediately
- Respect individuals' privacy rights and data subject requests
- Follow data retention and disposal policies

## GDPR Compliance
I understand the principles of GDPR including:
- Lawfulness, fairness, and transparency in data processing
- Purpose limitation and data minimization
- Accuracy and storage limitation requirements
- Security and accountability obligations

## Security Measures
I commit to:
- Using strong passwords and enabling multi-factor authentication
- Keeping software updated and following IT security policies
- Not sharing access credentials or leaving systems unattended
- Reporting suspicious activities or security concerns"
                },
                new()
                {
                    Id = "signature-documents/equipment-responsibility",
                    Title = "Company Equipment Responsibility Agreement",
                    Type = "Agreement",
                    Version = 1,
                    CreatedDate = new DateTime(2024, 2, 15),
                    CreatedBy = "IT Department",
                    Description = "Agreement outlining responsibilities for company-provided equipment",
                    Tags = new() { "equipment", "responsibility", "IT", "assets" },
                    Content = @"# Company Equipment Responsibility Agreement

## Equipment Assignment
I acknowledge receipt of company equipment as detailed in the attached inventory list and agree to:
- Use equipment solely for business purposes
- Maintain equipment in good working condition
- Report damage, loss, or theft immediately
- Allow IT department access for maintenance and updates
- Return all equipment in good condition upon request or termination

## Security and Care
I commit to:
- Installing only approved software and updates
- Using strong passwords and security features
- Protecting equipment from theft, damage, or unauthorized access
- Following IT policies for data backup and security
- Not attempting unauthorized repairs or modifications

## Liability and Return
I understand that I may be liable for damage due to negligence or misuse and that all equipment must be returned promptly when requested."
                },
                new()
                {
                    Id = "signature-documents/social-media-policy",
                    Title = "Social Media and Communications Policy",
                    Type = "Policy",
                    Version = 1,
                    CreatedDate = new DateTime(2024, 5, 1),
                    CreatedBy = "Marketing Department",
                    Description = "Guidelines for social media use and external communications representing the company",
                    Tags = new() { "social media", "communications", "brand", "policy" },
                    Content = @"# Social Media and Communications Policy

## Personal Social Media Use
When posting on personal social media accounts, I agree to:
- Not disclose confidential company information
- Include disclaimers when discussing work-related topics
- Respect colleagues' privacy and company reputation
- Follow professional standards of conduct online
- Separate personal opinions from company positions

## Company Representation
If authorized to represent the company online, I will:
- Follow brand guidelines and approved messaging
- Maintain professional tone and accuracy in all posts
- Coordinate with marketing team for official communications
- Respond promptly and appropriately to customer inquiries
- Escalate negative feedback or crises to appropriate teams

## Compliance and Monitoring
I understand that social media activity may be monitored for compliance and that violations may result in disciplinary action."
                }
            };

            foreach (var document in documents)
            {
                await session.StoreAsync(document);
            }
        }
    }
}
