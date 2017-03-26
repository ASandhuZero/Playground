(ns clojure-solutions.core
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
    (action something))
  (def hello-print (partial hello println))
  (hello-print "Alright partials are really cool. I want to do more functional programming")
)
