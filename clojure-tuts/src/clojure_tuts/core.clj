(ns clojure-tuts.core
  (:require [clojure-tuts.core :as tuts])
  (:gen-class))

  (defn -example
    []
    "Just some random function"
    (println (str "Hello" " World"))
    (println (+ 1 2 )))
    (println *ns*) ; This namespace is clojure-tuts.core.
; (-example)
(defn -main
  "I don't do a whole lot ... yet."
  [& args]
  ; (-example) I wonder if there is a way to get a function in there.
  ; (clojure-tuts.core.-example) This doesn't work because namespaces.
  ; (tuts.-example)
  (println *ns*) ; This namespace is user! Not clojure-tuts.core!
  (println (find-ns 'clojure-tuts.core))
  (println (all-ns))
  (defn example1
    []
    (println "This runs because namespaces!"))
  (example1)
  (println "Hello, World!"))
