using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPlus.Model
{
    public class WorkflowHistory
    {
        UserInfo _UserInfo = new UserInfo();
        private List<WorkflowStep> steps = new List<WorkflowStep>();
        private List<RequestStep> requestSteps = new List<RequestStep>();
        public int ID
        {
            set;
            get;
        }

        public List<WorkflowStep> Steps 
        { 
            get
            {
                return steps;
            }
            set
            {
                steps = value;
            }
        }

        public List<RequestStep> RequestSteps
        {
            get
            {
                return requestSteps;
            }
            set
            {
                requestSteps = value;
            }
        }
    }
}
