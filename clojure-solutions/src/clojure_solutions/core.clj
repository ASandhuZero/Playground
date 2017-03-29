(ns clojure-solutions.core
  ; (:require [clojure-solutions.Easy-277])
  ; (:import [clojure-solutions.Easy-277 hello-277]))
  (:gen-class))

(defn -main
  "I don't do a whole lot ... yet."
  [& args]
  (def action println)
  (action "Hello, World")
  (def a "I am learning about Clojure and functional programming!")
  (action a)

  (defn hello
    [action,something]
    ; (hello-277 "Hi")
    (action something))
  (def hello-print (partial hello println))
  (hello-print "Alright partials are really cool. I want to do more functional programming")
)
