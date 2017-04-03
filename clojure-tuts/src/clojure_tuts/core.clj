(ns clojure-tuts.core
  (:gen-class))

  (defn -example
    []
    "Just some random function"
    (println (str "Hello" " World"))
    (println (+ 1 2 )))
; (-example)
(defn -main
  "I don't do a whole lot ... yet."
  [& args]
  ; (-example) I wonder if there is a way to get a function in there.
  (clojure-tuts.core.-example)
  (println "Hello, World!"))
