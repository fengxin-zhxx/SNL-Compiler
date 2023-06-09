﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CompilationPrinciple {
    public enum LexType {
        ENDFILE, ERROR,
        //Reserved words
        PROGRAM, PROCEDURE, TYPE, VAR, IF,
        THEN, ELSE, FI, WHILE, DO,
        ENDWH, BEGIN, END, READ, WRITE,
        ARRAY, OF, RECORD, RETURN,
        //Type
        INTEGER_T, CHAR_T,
        //Multi-Character Word Symbols
        ID, INTC_VAL, CHARC_VAL,
        //Special Symbols
        ASSIGN, EQ, LT, PLUS, MINUS,
        TIMES, DIVIDE, LPAREN, RPAREN, DOT,
        COLON, SEMI, COMMA, LMIDPAREN, RMIDPAREN,
        UNDERRANGE,
        //Non-Terminators
        Program, ProgramHead, ProgramName, DeclarePart,
        TypeDec, TypeDeclaration, TypeDecList, TypeDecMore,
        TypeId, TypeName, BaseType, StructureType,
        ArrayType, Low, Top, RecType,
        FieldDecList, FieldDecMore, IdList, IdMore,
        VarDec, VarDeclaration, VarDecList, VarDecMore,
        VarIdList, VarIdMore, ProcDec, ProcDeclaration,
        ProcDecMore, ProcName, ParamList, ParamDecList,
        ParamMore, Param, FormList, FidMore,
        ProcDecPart, ProcBody, ProgramBody, StmList,
        StmMore, Stm, AssCall, AssignmentRest,
        ConditionalStm, StmL, LoopStm, InputStm,
        InVar, OutputStm, ReturnStm, CallStmRest,
        ActParamList, ActParamMore, RelExp, OtherRelE,
        Exp, OtherTerm, Term, OtherFactor,
        Factor, Variable, VariMore, FieldVar,
        FieldVarMore, CmpOp, AddOp, MultOp
    };
    

    public enum LexStatus {
        START, INID, INNUM, DONE, INASSIGN, INCOMMENT, INRANGE, INCHAR, ERROR, FINISH
    }

    public enum LexUnit {
        Letter, Number, SingleSep, AssignFirst, AssignSecond, LeftCurly, RightCurly, Dot, SingleQuo, Space
    }

    public class Token {
        public int line { get; set; } 
        public int column { get; set; } 
        public LexType lex { get; set; } // Lexical Type of token
        public String sem { get; set; } // Semantic Info of token
    }

    public class Dictionarys {
        public Dictionarys() {
            //Provide the conversion from identifier to identifier type 
            reservedWords = new Dictionary<String, LexType>()
            {
                {"program",LexType.PROGRAM},{"type", LexType.TYPE},
                {"var", LexType.VAR},         {"procedure", LexType.PROCEDURE},
                {"begin", LexType.BEGIN},     {"end", LexType.END},
                {"array", LexType.ARRAY},     {"of", LexType.OF},
                {"record", LexType.RECORD},   {"if", LexType.IF},
                {"then", LexType.THEN},       {"else",LexType.ELSE},
                {"fi", LexType.FI},           {"char", LexType.CHAR_T},
                {"while", LexType.WHILE},     {"do", LexType.DO},
                {"endwh", LexType.ENDWH},     {"read", LexType.READ},
                {"write", LexType.WRITE},     {"return", LexType.RETURN},
                {"integer", LexType.INTEGER_T},
            };

            //Provide the conversion from separatorWords to separatorWords type 
            separatorWords = new Dictionary<char, LexType>() {
                {'+', LexType.PLUS},   {'-', LexType.MINUS},     {'*', LexType.TIMES},
                {'/', LexType.DIVIDE}, {'(', LexType.LPAREN},    {')', LexType.RPAREN},
                {';', LexType.SEMI},   {'[', LexType.LMIDPAREN}, {']', LexType.RMIDPAREN},
                {'=', LexType.EQ},     {'<', LexType.LT},        {',', LexType.COMMA}
            };
        }

        public Dictionary<String, LexType> reservedWords;

        public Dictionary<char, LexType> separatorWords;
    }

    //TODO  : Finish The Design Of State Transition Table 
    public class ConvertTable {
        int[,] convertTable;

        public ConvertTable() {

            convertTable = new int[8, 10];

            convertTable[(int)LexStatus.START, (int)LexUnit.Letter] = (int)LexStatus.INID;
            convertTable[(int)LexStatus.START, (int)LexUnit.Number] = (int)LexStatus.INNUM;
            convertTable[(int)LexStatus.START, (int)LexUnit.SingleSep] = (int)LexStatus.DONE;
            convertTable[(int)LexStatus.START, (int)LexUnit.AssignFirst] = (int)LexStatus.INASSIGN;
            convertTable[(int)LexStatus.START, (int)LexUnit.LeftCurly] = (int)LexStatus.INCOMMENT;
            convertTable[(int)LexStatus.START, (int)LexUnit.Dot] = (int)LexStatus.INRANGE;
            convertTable[(int)LexStatus.START, (int)LexUnit.SingleQuo] = (int)LexStatus.INCHAR;
            convertTable[(int)LexStatus.START, (int)LexUnit.Space] = (int)LexStatus.START;

            convertTable[(int)LexStatus.INID, (int)LexUnit.Letter] = (int)LexStatus.INID;
            convertTable[(int)LexStatus.INID, (int)LexUnit.Number] = (int)LexStatus.INID;
            convertTable[(int)LexStatus.INID, (int)LexUnit.SingleSep] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INID, (int)LexUnit.AssignFirst] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INID, (int)LexUnit.LeftCurly] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INID, (int)LexUnit.Dot] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INID, (int)LexUnit.SingleQuo] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INID, (int)LexUnit.Space] = (int)LexStatus.START;

            convertTable[(int)LexStatus.INNUM, (int)LexUnit.Letter] = (int)LexStatus.ERROR;
            convertTable[(int)LexStatus.INNUM, (int)LexUnit.Number] = (int)LexStatus.INNUM;
            convertTable[(int)LexStatus.INNUM, (int)LexUnit.SingleSep] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INNUM, (int)LexUnit.AssignFirst] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INNUM, (int)LexUnit.LeftCurly] = (int)LexStatus.ERROR;
            convertTable[(int)LexStatus.INNUM, (int)LexUnit.Dot] = (int)LexStatus.START;
            convertTable[(int)LexStatus.INNUM, (int)LexUnit.SingleQuo] = (int)LexStatus.ERROR;
            convertTable[(int)LexStatus.INNUM, (int)LexUnit.Space] = (int)LexStatus.START;
        }
    }
}
