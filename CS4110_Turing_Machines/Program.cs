/*
 *              Lawrence Magana
 *              CS 4110 - 7:30am
 *              HW3 - Turing Machines
 *              Dr. Rague
 *              Due: 4/13/18
 * 
 */
using System;
using System.Text;

namespace CS4110_Turing_Machines
{
    /// <summary>
    /// Class that creates the processing for a turing machine
    /// which takes a string input and determines if the word
    /// is a part of the language based on the turing machine's programs
    /// </summary>
    public class TuringMachine
    {
        //Stringbuilder used for string manipulation
        StringBuilder sb;

        /// Holds the set of programs for the TM
        /// State transition function as Code
        public string[] Code { get; set; }

        /// Array of State Types - 'H' if Halt State
        public char[] StateType { get; set; }

        /// Read/Write tape (input string is data)
        public string Tape { get; set; }

        //Tape head position
        public int TapePosition { get; set; }

        //Current state
        public int CurrentState { get; set; }

        public TuringMachine(string[] code, char[] statetype)
        {
            Code = code;
            StateType = statetype;
            SetState(0);
            TapePosition = 0;
        }

        public int GetState() { return CurrentState; }
        public void SetState(int state) { CurrentState = state; }

        /// <summary>
        /// Processes the next character from the tape by looping through the set of instructions (Code)
        /// to verify if a next state is available
        /// 
        // Instruction format:
        // "state1,inSymbol=>outSymbol,moveLR,state2"
        // "0,a=>A,R,1"
        // "0123456789"
        /// </summary>
        /// <param name="inChar"></param>
        public void StateTransition(char inChar)
        {
            //Loops through the array of instructions to find an existing branch from the current state
            string BranchingProgram = string.Empty;
            foreach (string program in Code)
            {
                int.TryParse(program[0].ToString(), out int ProgramState);
                char ProgramChar = program[2];
                if (ProgramState == GetState() && ProgramChar == inChar)
                {
                    BranchingProgram = program;
                    break;
                }
            }

            //If a program is found
            if (!string.IsNullOrWhiteSpace(BranchingProgram))
            {
                sb = new StringBuilder(Tape);

                //Replacement Letter
                char replacementChar = BranchingProgram[5];
                //Direction
                char direction = BranchingProgram[7];
                //Replace current letter w/ replacement letter
                sb[TapePosition] = replacementChar;

                Tape = sb.ToString();

                //If Direction = R, move tape + 1
                if (direction == 'R')
                {
                    TapePosition++;
                }
                //else if Direction == L, check and tape - 1
                else if (direction == 'L')
                {
                    if (TapePosition < 1)
                    {
                        StateType[GetState()] = 'C';
                        return;
                    }
                    else
                    {
                        TapePosition--;
                    }
                }

                //Sets the new state
                int.TryParse(BranchingProgram[9].ToString(), out int NewState);
                SetState(NewState);

            }
            else
            {
                StateType[GetState()] = 'C';
            }

        }

        /// <summary>
        /// Takes an input string and processes the word to determine
        /// whether it's a part of the language based on the TM's program
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public bool ProcessData(string inString)
        {
            SetState(0);
            TapePosition = 0;
            Tape = inString + "#";

            //While the current state is 'R', continue transititing the state
            while (StateType[GetState()] == 'R')
            {
                StateTransition(Tape[TapePosition]);
            }

            //If the current state was halted (accepted), return true
            if (StateType[GetState()] == 'H')
            {
                return true;
            }
            else if (StateType[GetState()] == 'C')
            {
                StateType[GetState()] = 'R';
                return false;
            }
            return false;
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            string word = string.Empty;

            /* Example */
            Console.WriteLine("--- Example --- Language consisting of all words that have 'b' as a second letter");
            string[] exCode =
            {
                "0,a=>a,R,1",
                "0,b=>b,R,1",
                "1,b=>b,R,2",
                "2,a=>a,R,2",
                "2,b=>b,R,2",
                "2,#=>#,R,3"
            };
            char[] exStateType = { 'R', 'R', 'R', 'H' };
            TuringMachine exTM = new TuringMachine(exCode, exStateType);
            Console.WriteLine("X = abaa\t Member? " + exTM.ProcessData("abaa"));
            Console.WriteLine("X = bbbb\t Member? " + exTM.ProcessData("abbb"));
            Console.WriteLine("X = aaaa\t Member? " + exTM.ProcessData("aaaa"));
            Console.WriteLine("X = babb\t Member? " + exTM.ProcessData("babb"));
            Console.WriteLine();

            /* Question 1 */
            Console.WriteLine("--- Question 1 --- Language consisting of all words that contain 'bbb'");
            string[] tm1Code =
            {
                "0,a=>a,R,0",
                "0,b=>b,R,1",
                "1,a=>a,R,0",
                "1,b=>b,R,2",
                "2,a=>a,R,0",
                "2,b=>b,R,3",
                "3,a=>a,R,3",
                "3,b=>b,R,3",
                "3,#=>#,R,4",
            };
            char[] tm1StateType = { 'R', 'R', 'R', 'R', 'H' };
            TuringMachine tm1 = new TuringMachine(tm1Code, tm1StateType);
            Console.WriteLine("X = abba\t Member? " + tm1.ProcessData("abba"));
            Console.WriteLine("X = abbba\t Member? " + tm1.ProcessData("abbba"));
            Console.WriteLine("X = bbababb\t Member? " + tm1.ProcessData("bbababb"));
            Console.WriteLine("X = babbbb\t Member? " + tm1.ProcessData("babbbb"));
            Console.WriteLine("X = aaabbb\t Member? " + tm1.ProcessData("aaabbb"));
            Console.WriteLine("X = bbbaaa\t Member? " + tm1.ProcessData("bbbaaa"));
            do
            {
                Console.Write("Q1 (-1 to exit): ");
                word = Console.ReadLine();
                Console.WriteLine($"X = {word}\tMember? {tm1.ProcessData(word)}");
            } while (word != "-1");
            Console.WriteLine();


            /* Question 2 */
            Console.WriteLine("--- Question 2 --- Language consisting of all words that have N 'b's\nfollowed by N+1 'a's where N > 0");
            string[] tm2Code =
            {
                "0,b=>B,R,1",
                "1,b=>b,R,1",
                "1,A=>A,R,1",
                "1,a=>A,L,2",
                "2,A=>A,L,2",
                "2,b=>b,L,3",
                "3,b=>b,L,3",
                "3,B=>B,R,0",
                "2,B=>B,R,4",
                "4,A=>A,R,4",
                "4,a=>a,R,5",
                "5,#=>#,R,6",
            };
            char[] tm2StateType = { 'R', 'R', 'R', 'R', 'R', 'R', 'H' };
            TuringMachine tm2 = new TuringMachine(tm2Code, tm2StateType);
            Console.WriteLine("X = baa \t Member? " + tm2.ProcessData("baa"));
            Console.WriteLine("X = bbbaa\t Member? " + tm2.ProcessData("bbbaa"));
            Console.WriteLine("X = bbaaaa\t Member? " + tm2.ProcessData("bbaaaa"));
            Console.WriteLine("X = bbaabb\t Member? " + tm2.ProcessData("bbaabb"));
            Console.WriteLine("X = bbaaa\t Member? " + tm2.ProcessData("bbaaa"));
            do
            {
                Console.Write("Q2 (-1 to exit): ");
                word = Console.ReadLine();
                Console.WriteLine($"X = {word}\tMember? {tm2.ProcessData(word)}");
            } while (word != "-1");
            Console.WriteLine();


            /* Question 3 */
            Console.WriteLine("--- Question 3 --- Language consisting of all odd palindromes");
            string[] tm3Code =
            {
                //"1,#=>#,R,8", //Accept - even palindromes
                "0,x=>#,R,1",
                "1,x=>x,R,1",
                "1,y=>y,R,1",
                "1,#=>#,L,2",
                "2,x=>#,L,3",
                "2,#=>#,R,7", //Accept
                "3,x=>x,L,3",
                "3,y=>y,L,3",
                "3,#=>#,R,0",
                "0,y=>#,R,4",
                "4,x=>x,R,4",
                "4,y=>y,R,4",
                "4,#=>#,L,5",
                "5,#=>#,R,7", //Accept
                "5,y=>#,L,6",
                "6,x=>x,L,6",
                "6,y=>y,L,6",
                "6,#=>#,R,0",
            };
            char[] tm3StateType = { 'R', 'R', 'R', 'R', 'R', 'R', 'R', 'H' };
            TuringMachine tm3 = new TuringMachine(tm3Code, tm3StateType);
            Console.WriteLine("X = xyx \t Member? " + tm3.ProcessData("xyx"));
            Console.WriteLine("X = yxxy\t Member? " + tm3.ProcessData("yxxy"));
            Console.WriteLine("X = xyyxy\t Member? " + tm3.ProcessData("xyyxy"));
            Console.WriteLine("X = xyyxy\t Member? " + tm3.ProcessData("xyyxy"));
            Console.WriteLine("X = yxyxyxy\t Member? " + tm3.ProcessData("yxyxyxy"));
            do
            {
                Console.Write("Q3 (-1 to exit): ");
                word = Console.ReadLine();
                Console.WriteLine($"X = {word}\tMember? {tm3.ProcessData(word)}");
            } while (word != "-1");
            Console.WriteLine();


            /* Question 4 */
            Console.WriteLine("--- Question 4 --- Language = q^np^nq^n where N > 0");
            string[] tm4Code =
            {
                "0,q=>*,R,1",
                "1,q=>q,R,1",
                "1,p=>p,R,2",
                "2,p=>p,R,2",
                "2,q=>q,L,3",
                "3,p=>q,R,4",
                "4,q=>q,R,4",
                "4,#=>#,L,5",
                "5,q=>#,L,6",
                "6,q=>#,L,7",
                "7,q=>q,L,7",
                "7,p=>p,L,7",
                "7,*=>*,R,0",
                "0,#=>#,R,8",
            };
            char[] tm4StateType = { 'R', 'R', 'R', 'R', 'R', 'R', 'R', 'R', 'H' };
            TuringMachine tm4 = new TuringMachine(tm4Code, tm4StateType);
            Console.WriteLine("X = qpq \t Member? " + tm4.ProcessData("qpq"));
            Console.WriteLine("X = qqpqq\t Member? " + tm4.ProcessData("qqpqq"));
            Console.WriteLine("X = qqppq\t Member? " + tm4.ProcessData("qqppq"));
            Console.WriteLine("X = qqppqq\t Member? " + tm4.ProcessData("qqppqq"));
            Console.WriteLine("X = qqppqqq\t Member? " + tm4.ProcessData("qqppqqq"));
            do
            {
                Console.Write("Q4 (-1 to exit): ");
                word = Console.ReadLine();
                Console.WriteLine($"X = {word}\tMember? {tm4.ProcessData(word)}");
            } while (word != "-1");
            Console.WriteLine();

            Console.ReadLine();
        }
    }
}

/*
 *CS4110 Assignment #3

Suggested Pseudocode for processData() and stateTransition() methods of TM class:

Consider: stateType array indicates if a state is a Halt state ('H') or effectively a Reject (potential Crash) state ('R')
Consider: crash occurs if (a) no outgoing transition exists for a given state or (b) the tape head is moved Left from the leftmost cell position on the tape

processData(inString : String) : boolean
	Place input string on tape (remember to place at least one "delta" character (#) at the end of the string)
	while current state is 'R'
		Call stateTransition(character at current tape head position)
	end while
	
	if(stateType of current state is 'H')
		return true;
	else
		return false;  <-- Crashes!
end processData
-
stateTransition(inChar: char) : void
	for each instruction in Code array
		search for an existing branch from current state
	end for

	if branching instruction exists
         	write new character at current tape head position
		move tape head to new location (check for crash!)
		set the machine to the new state
	else
		machine crashes 
end stateTransition
*/
