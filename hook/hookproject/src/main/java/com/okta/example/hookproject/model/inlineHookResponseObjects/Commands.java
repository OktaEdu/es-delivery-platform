package com.okta.example.hookproject.model.inlineHookResponseObjects;

public class Commands {
    String type;
    Value value;

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public Value getValue() {
        return value;
    }

    public void setValue(Value value) {
        this.value = value;
    }

//    public Commands(String type, Value value) {
//        this.type = type;
//        this.value = value;
//    }
}
